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
        Task<TossPayment?> ConfirmAsync(TossPostPaymentConfirm requst, string? IdempotencyKey = null);
        Task<TossPayment?> CancelAsync(string paymentKey, TossPostPaymentCancel postData, string? IdempotencyKey = null);
        string ClientKey { get; }
    }

}
