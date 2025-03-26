using kwangho.context;
using kwangho.mvc.Models;
using kwangho.tosspay;
using kwangho.tosspay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace kwangho.mvc.Controllers
{
    /// <summary>
    /// 주문 관련 컨트롤러
    /// </summary>
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IDistributedCache _cache;
        private readonly ITossPaymentService _tossPay;

        public OrderController(ILogger<OrderController> logger, ApplicationDbContext dbContext, IDistributedCache cache, ITossPaymentService tossPayment) : base(logger, dbContext)
        {
            _cache = cache;
            _tossPay = tossPayment;
        }

        /// <summary>
        /// 주문 페이지 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewBag.CliendId = _tossPay.ClientKey;
            return View();
        }

        /// <summary>
        /// 주문 처리
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<OrderPay>> Process(double amount, TossRequestMethod payMethod = TossRequestMethod.CARD)
        {
            var now = DateTime.UtcNow;
            var orderid = Guid.NewGuid().ToString();
            try
            {
                var model = new OrderPay
                {
                    CustomerKey = User.Identity?.Name,
                    TossRequest = new()
                    {
                        Method = payMethod,
                        Amount = new() { Value = amount },
                        OrderId = orderid[..12],
                        OrderName = "테스트 주문",
                        SuccessUrl = Url.ActionLink("TossPaySuccess", "Order")!,
                        FailUrl = Url.ActionLink("TossPayFail", "Order")!,
                        CustomerName = User.Identity?.Name
                    },
                    IdempotencyKey = orderid
                };
                //결제 성공 또는 실패시 주문정보를 찾기 위한 캐시 저장
                await _cache.SetAsync<OrderPay>(model.TossRequest.OrderId, model);
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ControllerContext.ActionDescriptor.ActionName);
            }
            return BadRequest();
        }
        /// <summary>
        /// 결제 성공시 호출
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> TossPaySuccess(string paymentType, string paymentKey, string orderId, double amount)
        {
            //기본값 실패
            var action = "TossPayFail";
            var controller = "Order";

            try
            {
                //cache 데이터 읽기
                if (_cache.TryGetValue<OrderPay>(orderId, out OrderPay? orderPayInfo))
                {
                    //결제 금액 확인 
                    if (orderPayInfo!.TossRequest!.Amount.Value != amount)
                        return RedirectToAction(action, controller, new { orderId, code="-1", message = "결제금액불일치" });

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
                        return View(orderPayInfo);
                    }
                    else
                        return RedirectToAction(action, controller, new { orderId, code = "-1", message = "승인요청 실패" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ControllerContext.ActionDescriptor.ActionName);
            }
            return RedirectToAction(action, controller, new { orderId, code = "-1", message = "결제정보없음" });
        }

        /// <summary>
        /// 결제 실패시 호출
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> TossPayFail(string code, string message, string orderId)
        {
            //기본값 실패
            IActionResult result = RedirectToAction("index", "Home");
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

                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ControllerContext.ActionDescriptor.ActionName);
            }
            return result;
        }

        /// <summary>
        /// 가상계좌 입금 완료시 호출되는
        /// Toss 결제 Callback (WebHook)
        /// 결제 콜백 경로는 Toss 상점 관리 페이지에 등록해야 호출 가능함
        /// 등록URL: {도메인URL}/Order/TossPayDepositCallback 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TossPayDepositCallback()
        {
            try
            {   //Toss 결제 Callback 처리
                TossWebHookDeposit? tossData = null;
                //Request Body 읽기 이상하게 메소드 파라미터로 받으면 값을 못 읽는 경우가 있음.
                using (var reader = new StreamReader(Request.Body))
                {
                    var inputstring = await reader.ReadToEndAsync();
                    tossData = JsonSerializer.Deserialize<TossWebHookDeposit>(inputstring);
                }
                if (tossData != null)
                {
                    //등록된 웹훅 URL에 상점과 토스페이먼츠가 아닌 제 3자에 의한 잘못된 요청이 들어올 수 있습니다.
                    //토스페이먼츠 서버에서 돌아온 올바른 요청이라면 결제 승인 결과로 돌아온 Payment 객체의 secret 값과 가상계좌 웹훅 이벤트 본문으로 돌아온 secret 값이 같습니다.
                    // TossPayment.Secret == TossWebHookDeposit.Secret

                    var now = DateTime.Now;
                    if (DateTime.TryParse(tossData.CreatedAt, out DateTime createAt))
                        now = createAt;

                    //상태별 주문 처리
                    if (tossData.Status == "DONE") //입금완료
                    {
                    }
                    else if (tossData.Status == "WAITING_FOR_DEPOSIT") //입금대기
                    {
                    }
                    else if (tossData.Status == "CANCELED") //취소
                    {

                    }
                    //Toss 결제 Callback 처리 후 응답
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ControllerContext.ActionDescriptor.ActionName);

            }
            return BadRequest();
        }
    }
}
