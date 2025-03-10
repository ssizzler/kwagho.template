using System.Text.Json.Serialization;

namespace kwangho.tosspay.Models
{
    /// <summary>
    /// 가상계좌 관련 정보
    /// </summary>
    public class TossPaymentVirtualAccountInfo
    {
        /// <summary>
        /// 가상계좌 타입을 나타냅니다. 일반, 고정 중 하나
        /// </summary>
        [JsonPropertyName("accountType")]
        public string? AccountType { get; set; }

        /// <summary>
        /// 발급된 계좌 번호입니다. 최대 길이는 20자
        /// </summary>
        [JsonPropertyName("accountNumber")]
        public string? AccountNumber { get; set; }

        /// <summary>
        /// 가상계좌 은행 숫자 코드, https://docs.tosspayments.com/reference/codes#%EC%9D%80%ED%96%89-%EC%BD%94%EB%93%9C 
        /// </summary>
        [JsonPropertyName("bankCode")]
        public string? BankCode { get; set; }

        /// <summary>
        /// 가상계좌를 발급한 고객 이름입니다. 최대 길이는 100자
        /// </summary>
        [JsonPropertyName("customerName")]
        public string? CustomerName { get; set; }

        /// <summary>
        /// 입금 기한
        /// </summary>
        [JsonPropertyName("dueDate")]
        public string? DueDate { get; set; }

        /// <summary>
        /// 환불 처리 상태
        /// </summary>
        /// <remarks>
        /// - NONE: 환불 요청이 없는 상태입니다.
        /// - PENDING: 환불을 처리 중인 상태입니다.
        /// - FAILED: 환불에 실패한 상태입니다.
        /// - PARTIAL_FAILED: 부분 환불에 실패한 상태입니다.
        /// - COMPLETED: 환불이 완료된 상태입니다.
        /// </remarks>
        [JsonPropertyName("refundStatus")]
        public string? RefundStatus { get; set; }

        /// <summary>
        /// 가상계좌가 만료되었는지 여부
        /// </summary>
        [JsonPropertyName("expired")]
        public bool? Expired { get; set; }

        /// <summary>
        /// 정산 상태입니다. 정산이 아직 되지 않았다면 INCOMPLETED, 정산이 완료됐다면 COMPLETED
        /// </summary>
        [JsonPropertyName("settlementStatus")]
        public string? SettlementStatus { get; set; }
    }

}
