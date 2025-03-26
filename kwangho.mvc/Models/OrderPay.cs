using kwangho.tosspay.Models;

namespace kwangho.mvc.Models
{
    /// <summary>
    /// 주문결제 모델
    /// </summary>
    public class OrderPay
    {
        /// <summary>
        /// 주문의 고유 key
        /// 운영환경에서는 주문 고유한 키를 사용해야 함.
        /// 단순 테스트를 위해 Guid를 사용
        /// </summary>
        public string OrderId => IdempotencyKey;

        /// <summary>
        /// Toss 상의 IdempotencyKey 
        /// </summary>
        public string IdempotencyKey { get; set; } = null!;

        /// <summary>
        /// 고객 고유 key
        /// </summary>
        public string? CustomerKey { get; set; }

        /// <summary>
        /// 토스에 요청할 결제 정보
        /// </summary>
        public TossRequestPayment? TossRequest { get; set; }

        public TossRequestMethod Method { get; set; } = TossRequestMethod.CARD;
    }
}