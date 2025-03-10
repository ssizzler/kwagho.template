using System.Text.Json.Serialization;

namespace kwangho.tosspay.Models
{
   
#region Toss TossPayment Response model

    /// <summary>
    /// 기본 응답 메시지, 에러 발생시 대응
    /// </summary>
    public class TossResponseMessage
    {
        /// <summary>
        /// 에러 타입을 보여주는 에러 코드
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// 에러 메시지
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    /// <summary>
    /// 결제와 관련한 모든 API와 결제 API의 응답으로 돌아오는 Payment 객체
    /// </summary>
    public class TossPayment : TossResponseMessage
    {
        [JsonIgnore]
        public bool IsSuccess
        {
            get
            {
                return string.IsNullOrEmpty(Code);
            }
        }

        /// <summary>
        /// Payment 객체의 응답 버전, 날짜 기반
        /// </summary>
        [JsonPropertyName("version")]
        public string? Version { get; set; }

        /// <summary>
        /// 결제의 키 값입니다. 최대 길이는 200자입니다.
        /// </summary>
        [JsonPropertyName("paymentKey")]
        public string? PaymentKey { get; set; }

        /// <summary>
        /// 결제 타입 정보입니다. NORMAL(일반 결제), BILLING(자동 결제), BRANDPAY(브랜드페이) 중 하나입니다.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("orderId")]
        public string? OrderId { get; set; }

        [JsonPropertyName("orderName")]
        public string? OrderName { get; set; }

        /// <summary>
        /// 상점아이디(MID)
        /// </summary>
        [JsonPropertyName("mId")]
        public string? MId { get; set; }

        /// <summary>
        /// 결제할 때 사용한 통화 단위입니다. 원화인 KRW만 사용합니다
        /// </summary>
        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        /// <summary>
        /// 결제할 때 사용한 결제 수단입니다. 카드, 가상계좌, 간편결제, 휴대폰, 계좌이체, 문화상품권, 도서문화상품권, 게임문화상품권 중 하나입니다.
        /// </summary>
        [JsonPropertyName("method")]
        public string? Method { get; set; }

        /// <summary>
        /// 총 결제 금액
        /// </summary>
        [JsonPropertyName("totalAmount")]
        public double TotalAmount { get; set; }

        /// <summary>
        /// 취소할 수 있는 금액(잔고)
        /// </summary>
        [JsonPropertyName("balanceAmount")]
        public double? BalanceAmount { get; set; }

        /// <summary>
        /// 결제 처리 상태
        /// </summary>
        /// <remarks>
        /// - READY: 결제를 생성하면 가지게 되는 초기 상태 입니다. 인증 전까지는 READY 상태를 유지합니다.
        /// - IN_PROGRESS: 결제 수단 정보와 해당 결제 수단의 소유자가 맞는지 인증을 마친 상태입니다.결제 승인 API를 호출하면 결제가 완료됩니다.
        /// - WAITING_FOR_DEPOSIT: 가상계좌 결제 흐름에만 있는 상태로, 결제 고객이 발급된 가상계좌에 입금하는 것을 기다리고 있는 상태입니다.
        /// - DONE: 인증된 결제 수단 정보, 고객 정보로 요청한 결제가 승인된 상태입니다.
        /// - CANCELED: 승인된 결제가 취소된 상태입니다.
        /// - PARTIAL_CANCELED: 승인된 결제가 부분 취소된 상태입니다.
        /// - ABORTED: 결제 승인이 실패한 상태입니다.
        /// - EXPIRED: 결제 유효 시간 30분이 지나 거래가 취소된 상태입니다.IN_PROGRESS 상태에서 결제 승인 API를 호출하지 않으면 EXPIRED가 됩니다.
        /// </remarks>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// 결제가 일어난 날짜와 시간 정보입니다. ISO 8601 형식인 yyyy-MM-dd'T'HH:mm:ss±hh:mm
        /// </summary>
        [JsonPropertyName("requestedAt")]
        public string? RequestedAt { get; set; }

        /// <summary>
        /// 결제 승인이 일어난 날짜와 시간 정보입니다. ISO 8601 형식인 yyyy-MM-dd'T'HH:mm:ss±hh:mm
        /// </summary>
        [JsonPropertyName("approvedAt")]
        public string? ApprovedAt { get; set; }

        /// <summary>
        /// 에스크로 사용 여부입니다.
        /// </summary>
        [JsonPropertyName("useEscrow")]
        public bool? UseEscrow { get; set; }

        /// <summary>
        /// 마지막 거래의 키 값입니다. 한 결제 건의 승인 거래와 취소 거래를 구분하는데 사용됩니다. 예를 들어 결제 승인 후 부분 취소를 두 번 했다면 마지막 부분 취소 거래의 키 값이 할당됩니다. 최대 길이는 64자입니다.
        /// </summary>
        [JsonPropertyName("lastTransactionKey")]
        public string? LastTransactionKey { get; set; }

        /// <summary>
        /// 공급가액입니다.
        /// </summary>
        [JsonPropertyName("suppliedAmount")]
        public double? SuppliedAmount { get; set; }

        /// <summary>
        /// 부가세입니다. (결제 금액 amount - 면세 금액 taxFreeAmount) / 11 후 소수점 첫째 자리에서 반올림해서 계산
        /// </summary>
        [JsonPropertyName("vat")]
        public double? Vat { get; set; }

        /// <summary>
        /// 문화비(도서, 공연 티켓, 박물관·미술관 입장권 등) 지출 여부입니다. 계좌이체, 가상계좌를 사용할 때만 설정하세요.
        /// </summary>
        [JsonPropertyName("cultureExpense")]
        public bool? CultureExpense { get; set; }

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
        /// 결제 취소 이력이 담기는 배열
        /// </summary>
        [JsonPropertyName("cancels")]
        public List<TossPaymentCancel>? Cancels { get; set; } = null;

        /// <summary>
        /// 부분 취소 가능 여부입니다. 이 값이 false이면 전액 취소만 가능합니다.
        /// </summary>
        [JsonPropertyName("isPartialCancelable")]
        public bool? IsPartialCancelable { get; set; }

        /// <summary>
        /// 카드로 결제하면 제공되는 카드 관련 정보
        /// </summary>
        [JsonPropertyName("card")]
        public TossPaymentCardInfo? Card { get; set; } = null;

        /// <summary>
        /// 가상계좌로 결제하면 제공되는 가상계좌 관련 정보
        /// </summary>
        [JsonPropertyName("virtualAccount")]
        public TossPaymentVirtualAccountInfo? VirtualAccount { get; set; } = null;

        /// <summary>
        /// 가상계좌 웹훅이 정상적인 요청인지 검증하는 값입니다. 이 값이 가상계좌 웹훅 이벤트 본문으로 돌아온 secret과 같으면 정상적인 요청입니다. 최대 길이는 50자 이하여야 합니다.
        /// </summary>
        [JsonPropertyName("secret")]
        public string? Secret { get; set; }

        /// <summary>
        /// 휴대폰으로 결제하면 제공되는 휴대폰 결제 관련 정보
        /// </summary>
        [JsonPropertyName("mobilePhone")]
        public TossPaymentMobilePhoneInfo? MobilePhone { get; set; } = null;

        /// <summary>
        /// 계좌이체로 결제했을 때 이체 관련 정보
        /// </summary>
        [JsonPropertyName("transfer")]
        public TossPaymentTransferInfo? Transfer { get; set; } = null;

        /// <summary>
        /// 발행된 영수증을 확인할 수 있는 주소
        /// </summary>
        [JsonPropertyName("receipt")]
        public TossPaymentUrlInfo? Receipt { get; set; } = null;

        /// <summary>
        /// 결제창이 열리는 주소
        /// </summary>
        [JsonPropertyName("checkout")]
        public TossPaymentUrlInfo? Checkout { get; set; } = null;

        /// <summary>
        /// 간편결제 정보
        /// </summary>
        [JsonPropertyName("easyPay")]
        public TossPaymentEasyPayInfo? EasyPay { get; set; } = null;

        /// <summary>
        /// 결제한 국가 정보입니다. ISO-3166의 두 자리 국가 코드 형식
        /// </summary>
        [JsonPropertyName("country")]
        public string? Country { get; set; }

        /// <summary>
        /// 결제 실패 정보
        /// </summary>
        [JsonPropertyName("failure")]
        public TossCallBackPaymentFail? Failure { get; set; } = null;

        /// <summary>
        /// 현금영수증 정보
        /// </summary>
        [JsonPropertyName("cashReceipt")]
        public TossPaymentCashReceiptInfo? CashReceipt { get; set; } = null;

        /// <summary>
        /// 카드사의 즉시 할인 프로모션 정보입니다. 즉시 할인 프로모션이 적용됐을 때만 생성
        /// </summary>
        [JsonPropertyName("discount")]
        public TossPaymentDiscountInfo? Discount { get; set; } = null;
    }
    
    /// <summary>
    /// URL 정보 객채
    /// </summary>
    public class TossPaymentUrlInfo
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
    
    /// <summary>
    /// 카드사의 즉시 할인 프로모션 정보
    /// </summary>
    public class TossPaymentDiscountInfo
    {
        /// <summary>
        /// 카드사의 즉시 할인 프로모션을 적용한 금액
        /// </summary>
        [JsonPropertyName("amount")]
        public double? Amount { get; set; }
    }

    #endregion
}
