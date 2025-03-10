using System.Text.Json.Serialization;

namespace kwangho.tosspay.Models
{
    /// <summary>
    /// 결제 승인 Request body model
    /// Server 
    /// POST /v1/payments/confirm
    /// </summary>
    public class TossPostPaymentConfirm
    {
        /// <summary>
        /// 결제의 키 값입니다. 최대 길이는 200자입니다.
        /// </summary>
        [JsonPropertyName("paymentKey")]
        public string? PaymentKey { get; set; }

        [JsonPropertyName("orderId")]
        public string? OrderId { get; set; }

        /// <summary>
        /// 결제 금액
        /// </summary>
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
    }
}
