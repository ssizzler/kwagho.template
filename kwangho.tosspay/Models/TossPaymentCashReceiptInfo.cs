using System.Text.Json.Serialization;

namespace kwangho.tosspay.Models
{
    /// <summary>
    /// 현금영수증 정보
    /// </summary>
    public class TossPaymentCashReceiptInfo
    {
        /// <summary>
        /// 현금영수증의 키 값입니다. 최대 길이는 200자
        /// </summary>
        [JsonPropertyName("receiptKey")]
        public string? ReceiptKey { get; set; }

        /// <summary>
        /// 현금영수증의 종류입니다. 소득공제, 지출증빙 중 하나의 값
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// 현금영수증 처리된 금액
        /// </summary>
        [JsonPropertyName("amount")]
        public double? Amount { get; set; }

        /// <summary>
        /// 면세 처리된 금액
        /// </summary>
        [JsonPropertyName("taxFreeAmount")]
        public double? TaxFreeAmount { get; set; }

        /// <summary>
        /// 현금영수증 발급 번호입니다. 최대 길이는 9자
        /// </summary>
        [JsonPropertyName("issueNumber")]
        public string? IssueNumber { get; set; }

        /// <summary>
        /// 발행된 현금영수증을 확인할 수 있는 주소
        /// </summary>
        [JsonPropertyName("receiptUrl")]
        public string? ReceiptUrl { get; set; }
    }

}
