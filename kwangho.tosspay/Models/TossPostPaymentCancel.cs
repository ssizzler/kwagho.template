using System.Text.Json.Serialization;

namespace kwangho.tosspay.Models
{
    /// <summary>
    ///  결제 취소 Request body model
    /// </summary>
    public class TossPostPaymentCancel
    {
        /// <summary>
        ///  결제 승인 Request body model
        /// </summary>
        [JsonPropertyName("cancelReason")]
        public string? CancelReason { get; set; }

        /// <summary>
        /// 취소할 금액입니다. 값이 없으면 전액 취소
        /// </summary>
        [JsonPropertyName("cancelAmount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CancelAmount { get; set; }

        /// <summary>
        /// 결제 취소 후 금액이 환불될 계좌의 정보
        /// 가상계좌 결제에만 필수
        /// </summary>
        [JsonPropertyName("refundReceiveAccount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TossRefundReceiveAccount? RefundReceiveAccount { get; set; }

        /// <summary>
        /// 취소할 금액 중 면세 금액입니다. 값을 넣지 않으면 기본값인 0으로 설정 
        /// </summary>
        [JsonPropertyName("taxFreeAmount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TaxFreeAmount { get; set; }
    }

}
