using System.Text.Json.Serialization;

namespace kwangho.tosspay.Models
{
    #region 결제 요청 결과 모델

    /// <summary>
    /// 결제 요청에 성공 Response Model
    /// JavaScript Client
    /// </summary>
    public class TossCallBackPaymentSuccess
    {
        /// <summary>
        /// 결제의 키 값입니다. 최대 길이는 200자입니다.
        /// </summary>
        public string? PaymentKey { get; set; }

        public string? OrderId { get; set; }

        /// <summary>
        /// 결제 금액
        /// </summary>
        public int Amount { get; set; }
    }

    /// <summary>
    /// 결제 요청에 실패 Response Model
    /// https://docs.tosspayments.com/reference/error-codes#failurl%EB%A1%9C-%EC%A0%84%EB%8B%AC%EB%90%98%EB%8A%94-%EC%97%90%EB%9F%AC
    /// JavaScript Client
    /// </summary>
    public class TossCallBackPaymentFail : TossResponseMessage
    {
        public string? OrderId { get; set; }
    }

    #endregion

    #region 결제 승인, 취소 서버에서 사용 모델

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
        public string? paymentKey { get; set; }

        public string? orderId { get; set; }

        /// <summary>
        /// 결제 금액
        /// </summary>
        public int amount { get; set; }
    }

    /// <summary>
    ///  결제 취소 Request body model
    /// </summary>
    public class TossPostPaymentCancel
    {
        /// <summary>
        ///  결제 승인 Request body model
        /// </summary>
        public string? cancelReason { get; set; }

        /// <summary>
        /// 취소할 금액입니다. 값이 없으면 전액 취소
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? cancelAmount { get; set; }

        /// <summary>
        /// 결제 취소 후 금액이 환불될 계좌의 정보
        /// 가상계좌 결제에만 필수
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TossRefundReceiveAccount? refundReceiveAccount { get; set; }

        /// <summary>
        /// 취소할 금액 중 면세 금액입니다. 값을 넣지 않으면 기본값인 0으로 설정 
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? taxFreeAmount { get; set; }

    }

    /// <summary>
    /// 결제 취소 후 금액이 환불될 계좌의 정보
    /// 가상계좌 결제에만 필수
    /// </summary>
    public class TossRefundReceiveAccount
    {
        /// <summary>
        /// 취소 금액을 환불받을 계좌의 은행 코드 
        /// </summary>
        public string? bank { get; set; }

        /// <summary>
        /// 취소 금액을 환불받을 계좌의 계좌 번호 입니다. - 없이 숫자만 넣어야 합니다. 최대 길이는 20자
        /// </summary>
        public string? accountNumber { get; set; }

        /// <summary>
        /// 취소 금액을 환불받을 계좌의 예금주입니다. 최대 길이는 60자입니다.
        /// </summary>
        public string? holderName { get; set; }
    }

    /// <summary>
    /// 가상계좌 결제 상태를 알려주는 웹훅 모델
    /// </summary>
    public class TossWebHookDeposit
    {
        /// <summary>
        /// 웹훅이 생성된 시간입니다. ISO 8601 형식인 yyyy-MM-dd'T'HH:mm:ss.SSSSSS 사용
        /// </summary>
        [JsonPropertyName("createdAt")]
        public string? createdAt { get; set; }
        /// <summary>
        /// public string secret { get; set; }
        /// </summary>
        [JsonPropertyName("secret")]
        public string? secret { get; set; }

        /// <summary>
        /// 결제 상태
        /// </summary>
        [JsonPropertyName("status")]
        public string? status { get; set; }

        /// <summary>
        /// 상태가 변경된 가상계좌 거래를 특정하는 키
        /// </summary>
        [JsonPropertyName("transactionKey")]
        public string? transactionKey { get; set; }
        [JsonPropertyName("orderId")]
        public string? orderId { get; set; }
    }

    #endregion

    #region Toss TossPayment Response model

    /// <summary>
    /// 기본 응답 메시지, 에러 발생시 대응
    /// </summary>
    public class TossResponseMessage
    {
        /// <summary>
        /// 에러 타입을 보여주는 에러 코드
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// 에러 메시지
        /// </summary>
        public string? Message { get; set; }
    }

    /// <summary>
    /// 결제와 관련한 모든 API와 결제 API의 응답으로 돌아오는 Payment 객체
    /// </summary>
    public class TossPayment : TossResponseMessage
    {
        [JsonIgnore]
        public bool isSuccess
        {
            get
            {
                return string.IsNullOrEmpty(Code);
            }
        }
        /// <summary>
        /// Payment 객체의 응답 버전, 날짜 기반
        /// </summary>
        public string? version { get; set; }
        /// <summary>
        /// 결제의 키 값입니다. 최대 길이는 200자입니다.
        /// </summary>
        public string? paymentKey { get; set; }
        /// <summary>
        /// 결제 타입 정보입니다. NORMAL(일반 결제), BILLING(자동 결제), BRANDPAY(브랜드페이) 중 하나입니다.
        /// </summary>
        public string? type { get; set; }
        public string? orderId { get; set; }
        public string? orderName { get; set; }
        /// <summary>
        /// 상점아이디(MID)
        /// </summary>
        public string? mId { get; set; }
        /// <summary>
        /// 결제할 때 사용한 통화 단위입니다. 원화인 KRW만 사용합니다
        /// </summary>
        public string? currency { get; set; }
        /// <summary>
        /// 결제할 때 사용한 결제 수단입니다. 카드, 가상계좌, 간편결제, 휴대폰, 계좌이체, 문화상품권, 도서문화상품권, 게임문화상품권 중 하나입니다.
        /// </summary>
        public string? method { get; set; }
        /// <summary>
        /// 총 결제 금액
        /// </summary>
        public double totalAmount { get; set; }
        /// <summary>
        /// 취소할 수 있는 금액(잔고)
        /// </summary>
        public double? balanceAmount { get; set; }

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
        public string? status { get; set; }
        /// <summary>
        /// 결제가 일어난 날짜와 시간 정보입니다. ISO 8601 형식인 yyyy-MM-dd'T'HH:mm:ss±hh:mm
        /// </summary>
        public string? requestedAt { get; set; }
        /// <summary>
        /// 결제 승인이 일어난 날짜와 시간 정보입니다. ISO 8601 형식인 yyyy-MM-dd'T'HH:mm:ss±hh:mm
        /// </summary>
        public string? approvedAt { get; set; }
        /// <summary>
        /// 에스크로 사용 여부입니다.
        /// </summary>
        public bool? useEscrow { get; set; }
        /// <summary>
        /// 마지막 거래의 키 값입니다. 한 결제 건의 승인 거래와 취소 거래를 구분하는데 사용됩니다. 예를 들어 결제 승인 후 부분 취소를 두 번 했다면 마지막 부분 취소 거래의 키 값이 할당됩니다. 최대 길이는 64자입니다.
        /// </summary>
        public string? lastTransactionKey { get; set; }
        /// <summary>
        /// 공급가액입니다.
        /// </summary>
        public double? suppliedAmount { get; set; }
        /// <summary>
        /// 부가세입니다. (결제 금액 amount - 면세 금액 taxFreeAmount) / 11 후 소수점 첫째 자리에서 반올림해서 계산
        /// </summary>
        public double? vat { get; set; }
        /// <summary>
        /// 문화비(도서, 공연 티켓, 박물관·미술관 입장권 등) 지출 여부입니다. 계좌이체, 가상계좌를 사용할 때만 설정하세요.
        /// </summary>
        public bool? cultureExpense { get; set; }
        /// <summary>
        /// 결제 금액 중 면세 금액
        /// </summary>
        public double? taxFreeAmount { get; set; }
        /// <summary>
        /// 결제 금액 중 과세 제외 금액(컵 보증금 등)입니다.
        /// *과세 제외 금액이 있는 카드 결제는 부분 취소가 안 됩니다.
        /// </summary>
        public double? taxExemptionAmount { get; set; }
        /// <summary>
        /// 결제 취소 이력이 담기는 배열
        /// </summary>
        public List<TossPaymentCancel>? cancels { get; set; } = null;

        /// <summary>
        /// 부분 취소 가능 여부입니다. 이 값이 false이면 전액 취소만 가능합니다.
        /// </summary>
        public bool? isPartialCancelable { get; set; }
        /// <summary>
        /// 카드로 결제하면 제공되는 카드 관련 정보
        /// </summary>
        public TossPaymentCardInfo? card { get; set; } = null;
        /// <summary>
        /// 가상계좌로 결제하면 제공되는 가상계좌 관련 정보
        /// </summary>
        public TossPaymentVirtualAccountInfo? virtualAccount { get; set; } = null;
        /// <summary>
        /// 가상계좌 웹훅이 정상적인 요청인지 검증하는 값입니다. 이 값이 가상계좌 웹훅 이벤트 본문으로 돌아온 secret과 같으면 정상적인 요청입니다. 최대 길이는 50자 이하여야 합니다.
        /// </summary>
        public string? secret { get; set; }
        /// <summary>
        /// 휴대폰으로 결제하면 제공되는 휴대폰 결제 관련 정보
        /// </summary>
        public TossPaymentMobilePhoneInfo? mobilePhone { get; set; } = null;
        /// <summary>
        /// 계좌이체로 결제했을 때 이체 관련 정보
        /// </summary>
        public TossPaymentTransferInfo? transfer { get; set; } = null;

        /// <summary>
        /// 발행된 영수증을 확인할 수 있는 주소
        /// </summary>
        public TossPaymentUrlInfo? receipt { get; set; } = null;
        /// <summary>
        /// 결제창이 열리는 주소
        /// </summary>
        public TossPaymentUrlInfo? checkout { get; set; } = null;
        /// <summary>
        /// 간편결제 정보
        /// </summary>
        public TossPaymentEasyPayInfo? easyPay { get; set; } = null;
        /// <summary>
        /// 결제한 국가 정보입니다. ISO-3166의 두 자리 국가 코드 형식
        /// </summary>
        public string? country { get; set; }

        /// <summary>
        /// 결제 실패 정보
        /// </summary>
        public TossCallBackPaymentFail? failure { get; set; } = null;
        /// <summary>
        /// 현금영수증 정보
        /// </summary>
        public TossPaymentCashReceiptInfo? cashReceipt { get; set; } = null;
        /// <summary>
        /// 카드사의 즉시 할인 프로모션 정보입니다. 즉시 할인 프로모션이 적용됐을 때만 생성
        /// </summary>
        public TossPaymentDiscountInfo? discount { get; set; } = null;
    }

    /// <summary>
    /// 카드 관련 정보
    /// </summary>
    public class TossPaymentCardInfo
    {
        /// <summary>
        /// 카드로 결제한 금액
        /// </summary>
        public double amount { get; set; }
        /// <summary>
        /// 카드 발급사 숫자 코드, https://docs.tosspayments.com/reference/codes#%EC%B9%B4%EB%93%9C%EC%82%AC-%EC%BD%94%EB%93%9C
        /// </summary>
        public string? issuerCode { get; set; }
        /// <summary>
        /// 카드 매입사 숫자 코드
        /// </summary>
        public string? acquirerCode { get; set; }
        /// <summary>
        /// 카드번호입니다. 번호의 일부는 마스킹 
        /// </summary>
        public string? number { get; set; }
        /// <summary>
        /// 할부 개월 수입니다. 일시불이면 0
        /// </summary>
        public int installmentPlanMonths { get; set; }
        /// <summary>
        /// 카드사 승인 번호입니다. 최대 길이는 8자
        /// </summary>
        public string? approveNo { get; set; }
        /// <summary>
        /// 카드사 포인트를 사용했는지 여부
        /// </summary>
        public bool? useCardPoint { get; set; }
        /// <summary>
        /// 카드 종류입니다. 신용, 체크, 기프트 중 하나
        /// </summary>
        public string? cardType { get; set; }
        /// <summary>
        /// 카드의 소유자 타입입니다. 개인, 법인 중 하나
        /// </summary>
        public string? ownerType { get; set; }
        /// <summary>
        /// 카드 결제의 매입 상태
        /// </summary>
        /// <remarks>- READY: 아직 매입 요청이 안 된 상태입니다.
        /// - REQUESTED: 매입이 요청된 상태입니다.
        /// - COMPLETED: 요청된 매입이 완료된 상태입니다.
        /// - CANCEL_REQUESTED: 매입 취소가 요청된 상태입니다.
        /// - CANCELED: 요청된 매입 취소가 완료된 상태입니다.
        /// </remarks>
        public string? acquireStatus { get; set; }
        /// <summary>
        /// 무이자 할부의 적용 여부
        /// </summary>
        public bool? isInterestFree { get; set; }
        /// <summary>
        /// 무이자 할부가 적용된 결제에서 할부 수수료를 부담하는 주체
        /// </summary>
        /// <remarks>
        /// - BUYER: 상품을 구매한 고객이 할부 수수료를 부담합니다.
        /// - CARD_COMPANY: 카드사에서 할부 수수료를 부담합니다.
        /// - MERCHANT: 상점에서 할부 수수료를 부담합니다.
        /// </remarks>
        public string? interestPayer { get; set; }
    }

    /// <summary>
    /// 가상계좌 관련 정보
    /// </summary>
    public class TossPaymentVirtualAccountInfo
    {
        /// <summary>
        /// 가상계좌 타입을 나타냅니다. 일반, 고정 중 하나
        /// </summary>
        public string? accountType { get; set; }
        /// <summary>
        /// 발급된 계좌 번호입니다. 최대 길이는 20자
        /// </summary>
        public string? accountNumber { get; set; }
        /// <summary>
        /// 가상계좌 은행 숫자 코드, https://docs.tosspayments.com/reference/codes#%EC%9D%80%ED%96%89-%EC%BD%94%EB%93%9C 
        /// </summary>
        public string? bankCode { get; set; }
        /// <summary>
        /// 가상계좌를 발급한 고객 이름입니다. 최대 길이는 100자
        /// </summary>
        public string? customerName { get; set; }
        /// <summary>
        /// 입금 기한
        /// </summary>
        public string? dueDate { get; set; }
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
        public string? refundStatus { get; set; }
        /// <summary>
        /// 가상계좌가 만료되었는지 여부
        /// </summary>
        public bool? expired { get; set; }
        /// <summary>
        /// 정산 상태입니다. 정산이 아직 되지 않았다면 INCOMPLETED, 정산이 완료됐다면 COMPLETED
        /// </summary>
        public string? settlementStatus { get; set; }
    }

    /// <summary>
    /// 휴대폰 결제 관련 정보
    /// </summary>
    public class TossPaymentMobilePhoneInfo
    {
        /// <summary>
        /// 결제에 사용한 휴대폰 번호
        /// </summary>
        public string? customerMobilePhone { get; set; }
        /// <summary>
        /// 정산 상태입니다. 정산이 아직 되지 않았다면 INCOMPLETED, 정산이 완료됐다면 COMPLETED
        /// </summary>
        public string? settlementStatus { get; set; }
        /// <summary>
        /// 휴대폰 결제 내역 영수증을 확인할 수 있는 주소
        /// </summary>
        public string? receiptUrl { get; set; }
    }
    /// <summary>
    /// 계좌이체 관련 정보  
    /// </summary>
    public class TossPaymentTransferInfo
    {
        /// <summary>
        /// 은행 숫자 코드
        /// </summary>
        public string? bankCode { get; set; }
        /// <summary>
        /// 정산 상태입니다. 정산이 아직 되지 않았다면 INCOMPLETED, 정산이 완료됐다면 COMPLETED
        /// </summary>
        public string? settlementStatus { get; set; }

    }
    /// <summary>
    /// 간편결제 정보
    /// </summary>
    public class TossPaymentEasyPayInfo
    {
        /// <summary>
        /// 선택한 간편결제사 코드. 
        /// 참조: https://docs.tosspayments.com/reference/codes#%EA%B0%84%ED%8E%B8%EA%B2%B0%EC%A0%9C%EC%82%AC-%EC%BD%94%EB%93%9C
        /// </summary>
        public string? provider { get; set; }
        /// <summary>
        /// 간편결제 서비스에 등록된 카드, 계좌 중 하나로 결제한 금액
        /// </summary>
        public double? amount { get; set; }
        /// <summary>
        /// 간편결제 서비스의 적립 포인트나 쿠폰 등을 사용해서 즉시 할인된 금액
        /// </summary>
        public double? discountAmount { get; set; }

    }
    /// <summary>
    /// URL 정보 객채
    /// </summary>
    public class TossPaymentUrlInfo
    {
        public string? url { get; set; }
    }
    /// <summary>
    /// 결제 취소
    /// </summary>
    public class TossPaymentCancel
    {
        /// <summary>
        /// 결제를 취소한 금액
        /// </summary>
        public double cancelAmount { get; set; }
        /// <summary>
        /// 결제를 취소한 이유입니다. 최대 길이는 200자입니다.
        /// </summary>
        public string? cancelReason { get; set; }
        /// <summary>
        /// 결제 금액 중 면세 금액
        /// </summary>
        public double? taxFreeAmount { get; set; }
        /// <summary>
        /// 결제 금액 중 과세 제외 금액(컵 보증금 등)입니다.
        /// *과세 제외 금액이 있는 카드 결제는 부분 취소가 안 됩니다.
        /// </summary>
        public double? taxExemptionAmount { get; set; }
        /// <summary>
        /// 결제 취소 후 환불 가능한 잔액
        /// </summary>
        public double? refundableAmount { get; set; }
        /// <summary>
        /// 간편결제 서비스의 포인트, 쿠폰, 즉시할인과 같은 적립식 결제 수단에서 취소된 금액
        /// </summary>
        public double? easyPayDiscountAmount { get; set; }
        /// <summary>
        /// 결제 취소가 일어난 날짜와 시간 정보입니다. ISO 8601 형식인 yyyy-MM-dd'T'HH:mm:ss±hh:mm
        /// </summary>
        public string? canceledAt { get; set; }
        /// <summary>
        /// 취소 건의 키 값입니다. 여러 건의 취소 거래를 구분하는데 사용됩니다. 최대 길이는 64자
        /// </summary>
        public string? transactionKey { get; set; }
    }

    /// <summary>
    /// 현금영수증 정보
    /// </summary>
    public class TossPaymentCashReceiptInfo
    {
        /// <summary>
        /// 현금영수증의 키 값입니다. 최대 길이는 200자
        /// </summary>
        public string? receiptKey { get; set; }
        /// <summary>
        /// 현금영수증의 종류입니다. 소득공제, 지출증빙 중 하나의 값
        /// </summary>
        public string? type { get; set; }
        /// <summary>
        /// 현금영수증 처리된 금액
        /// </summary>
        public double? amount { get; set; }
        /// <summary>
        /// 면세 처리된 금액
        /// </summary>
        public double? taxFreeAmount { get; set; }
        /// <summary>
        /// 현금영수증 발급 번호입니다. 최대 길이는 9자
        /// </summary>
        public string? issueNumber { get; set; }
        /// <summary>
        /// 발행된 현금영수증을 확인할 수 있는 주소
        /// </summary>
        public string? receiptUrl { get; set; }
    }
    /// <summary>
    /// 카드사의 즉시 할인 프로모션 정보
    /// </summary>
    public class TossPaymentDiscountInfo
    {
        /// <summary>
        /// 카드사의 즉시 할인 프로모션을 적용한 금액
        /// </summary>
        public double? amount { get; set; }

    }

    #endregion
}
