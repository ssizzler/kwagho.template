using System.Text.Json.Serialization;

namespace kwangho.tosspay.Models
{
    /// <summary>
    /// 결제 취소 후 금액이 환불될 계좌의 정보
    /// 가상계좌 결제에만 필수
    /// </summary>
    public class TossRefundReceiveAccount
    {
        /// <summary>
        /// 취소 금액을 환불받을 계좌의 은행 코드 
        /// </summary>
        [JsonPropertyName("bank")]
        public string? Bank { get; set; }

        /// <summary>
        /// 취소 금액을 환불받을 계좌의 계좌 번호 입니다. - 없이 숫자만 넣어야 합니다. 최대 길이는 20자
        /// </summary>
        [JsonPropertyName("accountNumber")]
        public string? AccountNumber { get; set; }

        /// <summary>
        /// 취소 금액을 환불받을 계좌의 예금주입니다. 최대 길이는 60자입니다.
        /// </summary>
        [JsonPropertyName("holderName")]
        public string? HolderName { get; set; }
    }

}
