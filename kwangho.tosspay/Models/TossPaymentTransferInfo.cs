using System.Text.Json.Serialization;

namespace kwangho.tosspay.Models
{
    /// <summary>
    /// 계좌이체 관련 정보  
    /// </summary>
    public class TossPaymentTransferInfo
    {
        /// <summary>
        /// 은행 숫자 코드
        /// </summary>
        [JsonPropertyName("bankCode")] 
        public string? BankCode { get; set; }

        /// <summary>
        /// 정산 상태입니다. 정산이 아직 되지 않았다면 INCOMPLETED, 정산이 완료됐다면 COMPLETED
        /// </summary>
        [JsonPropertyName("settlementStatus")] 
        public string? SettlementStatus { get; set; }

    }
}
