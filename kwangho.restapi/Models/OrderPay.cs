using kwangho.tosspay.Models;

namespace kwangho.restapi.Models
{
    /// <summary>
    /// 주문결제 모델
    /// </summary>
    public class OrderPay
    {
        /// <summary>
        /// 주문의 고유 key
        /// 운영환경에서는 주문 고유한 키를 사용해야 함.
        /// </summary>
        public string OrderId => IdempotencyKey;

        /// <summary>
        /// Toss 상의 IdempotencyKey 이고
        /// </summary>
        public string IdempotencyKey { get; set; } = null!;

        /// <summary>
        /// 토스에 요청할 결제 정보
        /// </summary>
        public TossRequestPayment? TossRequest { get; set; }
    }
}
