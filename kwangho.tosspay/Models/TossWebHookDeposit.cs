using System.Text.Json.Serialization;

namespace kwangho.tosspay.Models
{
    /// <summary>
    /// 가상계좌 결제 상태를 알려주는 웹훅 모델
    /// </summary>
    public class TossWebHookDeposit
    {
        /// <summary>
        /// 웹훅이 생성된 시간입니다. ISO 8601 형식인 yyyy-MM-dd'T'HH:mm:ss.SSSSSS 사용
        /// </summary>
        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        /// <summary>
        /// public string secret { get; set; }
        /// </summary>
        [JsonPropertyName("secret")]
        public string? Secret { get; set; }

        /// <summary>
        /// 결제 상태
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// 상태가 변경된 가상계좌 거래를 특정하는 키
        /// </summary>
        [JsonPropertyName("transactionKey")]
        public string? TransactionKey { get; set; }

        [JsonPropertyName("orderId")]
        public string? OrderId { get; set; }
    }

}
