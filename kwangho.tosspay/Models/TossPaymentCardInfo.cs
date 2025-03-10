using System.Text.Json.Serialization;

namespace kwangho.tosspay.Models
{
    /// <summary>
    /// 카드 관련 정보
    /// </summary>
    public class TossPaymentCardInfo
    {
        /// <summary>
        /// 카드로 결제한 금액
        /// </summary>
        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        /// <summary>
        /// 카드 발급사 숫자 코드, https://docs.tosspayments.com/reference/codes#%EC%B9%B4%EB%93%9C%EC%82%AC-%EC%BD%94%EB%93%9C
        /// </summary>
        [JsonPropertyName("issuerCode")]
        public string? IssuerCode { get; set; }

        /// <summary>
        /// 카드 매입사 숫자 코드
        /// </summary>
        [JsonPropertyName("acquirerCode")]
        public string? AcquirerCode { get; set; }

        /// <summary>
        /// 카드번호입니다. 번호의 일부는 마스킹 
        /// </summary>
        [JsonPropertyName("number")]
        public string? Number { get; set; }

        /// <summary>
        /// 할부 개월 수입니다. 일시불이면 0
        /// </summary>
        [JsonPropertyName("installmentPlanMonths")]
        public int InstallmentPlanMonths { get; set; }

        /// <summary>
        /// 카드사 승인 번호입니다. 최대 길이는 8자
        /// </summary>
        [JsonPropertyName("approveNo")]
        public string? ApproveNo { get; set; }

        /// <summary>
        /// 카드사 포인트를 사용했는지 여부
        /// </summary>
        [JsonPropertyName("useCardPoint")]
        public bool? UseCardPoint { get; set; }

        /// <summary>
        /// 카드 종류입니다. 신용, 체크, 기프트 중 하나
        /// </summary>
        [JsonPropertyName("cardType")]
        public string? CardType { get; set; }

        /// <summary>
        /// 카드의 소유자 타입입니다. 개인, 법인 중 하나
        /// </summary>
        [JsonPropertyName("ownerType")]
        public string? OwnerType { get; set; }

        /// <summary>
        /// 카드 결제의 매입 상태
        /// </summary>
        /// <remarks>
        /// - READY: 아직 매입 요청이 안 된 상태입니다.
        /// - REQUESTED: 매입이 요청된 상태입니다.
        /// - COMPLETED: 요청된 매입이 완료된 상태입니다.
        /// - CANCEL_REQUESTED: 매입 취소가 요청된 상태입니다.
        /// - CANCELED: 요청된 매입 취소가 완료된 상태입니다.
        /// </remarks>
        [JsonPropertyName("acquireStatus")]
        public string? AcquireStatus { get; set; }

        /// <summary>
        /// 무이자 할부의 적용 여부
        /// </summary>
        [JsonPropertyName("isInterestFree")]
        public bool? IsInterestFree { get; set; }

        /// <summary>
        /// 무이자 할부가 적용된 결제에서 할부 수수료를 부담하는 주체
        /// </summary>
        /// <remarks>
        /// - BUYER: 상품을 구매한 고객이 할부 수수료를 부담합니다.
        /// - CARD_COMPANY: 카드사에서 할부 수수료를 부담합니다.
        /// - MERCHANT: 상점에서 할부 수수료를 부담합니다.
        /// </remarks>
        [JsonPropertyName("interestPayer")]
        public string? InterestPayer { get; set; }
    }
}
