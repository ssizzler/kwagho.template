using System.Text.Json.Serialization;

namespace kwangho.tosspay.Models
{
    /// <summary>
    /// 결제 취소
    /// </summary>
    public class TossPaymentCancel
    {
        /// <summary>
        /// 결제를 취소한 금액
        /// </summary>
        [JsonPropertyName("cancelAmount")]
        public double CancelAmount { get; set; }

        /// <summary>
        /// 결제를 취소한 이유입니다. 최대 길이는 200자입니다.
        /// </summary>
        [JsonPropertyName("cancelReason")]
        public string? CancelReason { get; set; }

        /// <summary>
        /// 결제 금액 중 면세 금액
        /// </summary>
        [JsonPropertyName("taxFreeAmount")]
        public double? TaxFreeAmount { get; set; }

        /// <summary>
        /// 결제 금액 중 과세 제외 금액(컵 보증금 등)입니다.
        /// *과세 제외 금액이 있는 카드 결제는 부분 취소가 안 됩니다.
        /// </summary>
        [JsonPropertyName("taxExemptionAmount")]
        public double? TaxExemptionAmount { get; set; }

        /// <summary>
        /// 결제 취소 후 환불 가능한 잔액
        /// </summary>
        [JsonPropertyName("refundableAmount")]
        public double? RefundableAmount { get; set; }

        /// <summary>
        /// 간편결제 서비스의 포인트, 쿠폰, 즉시할인과 같은 적립식 결제 수단에서 취소된 금액
        /// </summary>
        [JsonPropertyName("easyPayDiscountAmount")]
        public double? EasyPayDiscountAmount { get; set; }

        /// <summary>
        /// 결제 취소가 일어난 날짜와 시간 정보입니다. ISO 8601 형식인 yyyy-MM-dd'T'HH:mm:ss±hh:mm
        /// </summary>
        [JsonPropertyName("canceledAt")]
        public string? CanceledAt { get; set; }

        /// <summary>
        /// 취소 건의 키 값입니다. 여러 건의 취소 거래를 구분하는데 사용됩니다. 최대 길이는 64자
        /// </summary>
        [JsonPropertyName("transactionKey")]
        public string? TransactionKey { get; set; }
    }
}
