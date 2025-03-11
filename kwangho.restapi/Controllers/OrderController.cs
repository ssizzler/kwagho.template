using kwangho.context;
using kwangho.restapi.Models;
using kwangho.tosspay;
using kwangho.tosspay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace kwangho.restapi.Controllers
{

    /// <summary>
    /// 주문 관련 API
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    public class OrderController : BaseController
    {
        private readonly IDistributedCache _cache;
        private readonly ITossPaymentService _tossPay;

        public OrderController(ILogger<OrderController> logger, ApplicationDbContext dbContext, IDistributedCache cache, ITossPaymentService tossPayment)
            : base(logger, dbContext)
        {
            _cache = cache;
            _tossPay = tossPayment;
        }

        /// <summary>
        /// 입력된 금액으로 결제 정보를 생성
        /// </summary>
        /// <param name="amount">금액</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderPay>> Create(double amount, TossRequestMethod payMethod = TossRequestMethod.CARD)
        {
            ActionResult response = Unauthorized();
            try
            {

                var now = DateTime.UtcNow;
                var orderid = Guid.NewGuid().ToString();
                var model = new OrderPay
                {
                    TossRequest = new()
                    {
                        Method = payMethod,
                        Amount = new() { Value = amount },
                        OrderId = orderid[..12],
                        OrderName = "테스트 주문",
                        SuccessUrl = Url.ActionLink("TossPaySuccess", "Order")!,//API 프로젝트로 이 경로는 사용할 수 없음, 프론트엔트 경로로 설정해야함.
                        FailUrl = Url.ActionLink("TossPayFail", "Order")!,      //API 프로젝트로 이 경로는 사용할 수 없음, 프론트엔트 경로로 설정해야함.
                        CustomerName = User.Identity?.Name
                    },
                    IdempotencyKey = orderid
                };
                //결제 성공 또는 실패시 주문정보를 찾기 위한 캐시 저장
                await _cache.SetAsync<OrderPay>(model.IdempotencyKey, model);

                response = Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ControllerContext.ActionDescriptor.ActionName);
                response = BadRequest();
            }
            return response;
        }

        /// <summary>
        /// 결제 성공시 호출되는 API
        /// </summary>
        /// <param name="paymentKey"></param>
        /// <param name="orderId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TossPaySuccess")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> TossPaySuccess(string paymentKey, string orderId, double amount)
        {
            ActionResult response = BadRequest();
            try
            {
                //cache 데이터 읽기
                if (_cache.TryGetValue<OrderPay>(orderId, out OrderPay? orderPayInfo))
                {
                    //결제 금액 확인 
                    if (orderPayInfo!.TossRequest!.Amount.Value != amount)
                        return BadRequest("결제 금액 불일치.");

                    //승인요청
                    var toss = await _tossPay.Confirm(new TossPostPaymentConfirm
                    {
                        PaymentKey = paymentKey,
                        OrderId = orderId,
                        Amount = amount
                    }, orderPayInfo.IdempotencyKey);
                    if (toss == null)
                        return BadRequest("승인요청 실패");

                    //결제 승인 응답, 결재수단이 신용카드/ 계좌이체면 결제완료,  그 외 입금대기
                    if (toss.Status == "DONE" || toss.Status == "WAITING_FOR_DEPOSIT")
                    {
                        //주문 결제 정보 저장 처리

                        //만약 주문 처리 중 오류 발생시 결제 취소
                        if (false)
                        {
                            var canceltoss = await _tossPay.Cancel(paymentKey,
                                    new TossPostPaymentCancel
                                    {
                                        CancelReason = "주문 저장 실패",
                                    },
                                 orderPayInfo.IdempotencyKey);

                            if (canceltoss!.Status == "CANCELED")
                                return BadRequest("주문 오류로 결제 취소");
                            else
                                return BadRequest("주문 오류 및 결제 취소 실패");
                        }
                        
                        //완료시 cache 데이터 삭제
                        await _cache.RemoveAsync(orderId);
                        //결제 성공 처리
                        response = Ok(orderId);
                    }
                    else
                        return BadRequest("승인요청 실패");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ControllerContext.ActionDescriptor.ActionName);
                response = BadRequest();
            }
            return response;
        }

        /// <summary>
        /// 결제 실패시 호출되는 API
        /// </summary>
        /// <param name="code"></param>
        /// <param name="orderId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TossPayFail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> TossPayFail(string code, string orderId, string message)
        {
            ActionResult response = BadRequest();
            try
            {
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(orderId))
                {
                    //cache 데이터 읽기
                    if (_cache.TryGetValue<OrderPay>(orderId, out OrderPay? orderPayInfo))
                    {
                        //주문 실패 처리

                        //완료시 cache 데이터 삭제
                        await _cache.RemoveAsync(orderId);

                        response = Ok(orderId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ControllerContext.ActionDescriptor.ActionName);
                response = BadRequest();
            }
            return response;
        }
    }
}
