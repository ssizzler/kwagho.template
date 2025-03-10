using System.Text.Json.Serialization;

namespace kwangho.tosspay.Models
{
    /// <summary>
    /// 휴대폰 결제 관련 정보
    /// </summary>
    public class TossPaymentMobilePhoneInfo
    {
        /// <summary>
        /// 결제에 사용한 휴대폰 번호
        /// </summary>
        [JsonPropertyName("customerMobilePhone")]
        public string? CustomerMobilePhone { get; set; }

        /// <summary>
        /// 정산 상태입니다. 정산이 아직 되지 않았다면 INCOMPLETED, 정산이 완료됐다면 COMPLETED
        /// </summary>
        [JsonPropertyName("settlementStatus")]
        public string? SettlementStatus { get; set; }

        /// <summary>
        /// 휴대폰 결제 내역 영수증을 확인할 수 있는 주소
        /// </summary>
        [JsonPropertyName("receiptUrl")]
        public string? ReceiptUrl { get; set; }
    }

}
