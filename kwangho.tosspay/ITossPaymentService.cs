using kwangho.tosspay.Models;

namespace kwangho.tosspay
{
    /// <summary>
    /// Toss 결제 연동
    /// Version 2
    /// sdk문서: https://docs.tosspayments.com/sdk/v2/js#%EA%B2%B0%EC%A0%9C%EC%B0%BD
    /// sample: https://github.com/tosspayments/tosspayments-sample/blob/main/express-javascript/public/payment/checkout.html
    /// </summary>
    public interface ITossPaymentService
    {
        /// <summary>
        /// 결제 승인 호출
        /// </summary>
        Task<TossPayment?> Confirm(TossPostPaymentConfirm requst, string? IdempotencyKey = null);

        /// <summary>
        /// 결제 취소
        /// 결제 취소에 성공했다면 Payment 객체의 cancels 필드에 취소 객체가 배열로 돌아옵니다.
        /// </summary>
        Task<TossPayment?> Cancel(string paymentKey, TossPostPaymentCancel postData, string? IdempotencyKey = null);

        /// <summary>
        /// 승인된 결제를 paymentKey로 조회
        /// </summary>
        Task<TossPayment?> GetPayment(string paymentKey);

        /// <summary>
        /// 승인된 결제를 orderId로 조회
        /// </summary>
        Task<TossPayment?> GetPaymentFromOrderId(string orderId);

        /// <summary>
        /// 클라이언트키
        /// </summary>
        string ClientKey { get; }
    }

}
