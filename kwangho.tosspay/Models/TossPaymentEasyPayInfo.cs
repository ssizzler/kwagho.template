using System.Text.Json.Serialization;

namespace kwangho.tosspay.Models
{
    /// <summary>
    /// 간편결제 정보
    /// </summary>
    public class TossPaymentEasyPayInfo
    {
        /// <summary>
        /// 선택한 간편결제사 코드. 
        /// 참조: https://docs.tosspayments.com/reference/codes#%EA%B0%84%ED%8E%B8%EA%B2%B0%EC%A0%9C%EC%82%AC-%EC%BD%94%EB%93%9C
        /// </summary>
        [JsonPropertyName("provider")]
        public string? Provider { get; set; }

        /// <summary>
        /// 간편결제 서비스에 등록된 카드, 계좌 중 하나로 결제한 금액
        /// </summary>
        [JsonPropertyName("amount")]
        public double? Amount { get; set; }

        /// <summary>
        /// 간편결제 서비스의 적립 포인트나 쿠폰 등을 사용해서 즉시 할인된 금액
        /// </summary>
        [JsonPropertyName("discountAmount")]
        public double? DiscountAmount { get; set; }
    }

}
