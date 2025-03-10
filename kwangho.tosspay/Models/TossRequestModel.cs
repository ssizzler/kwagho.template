using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace kwangho.tosspay.Models
{
    #region Request model

    /// <summary>
    /// 결제 금액 정보
    /// </summary>
    public class TossAmount
    {
        /// <summary>
        /// 결제 통화
        /// </summary>
        public string Currency { get; set; } = "KRW";

        /// <summary>
        /// 결제 금액
        /// </summary>
        public double Value { get; set; }
    }

    /// <summary>
    /// 결제 수단
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TossRequestMethod
    {
        CARD,
        VIRTUAL_ACCOUNT,
        TRANSFER,
        MOBILE_PHONE
    }

    /// <summary>
    /// 가상계좌 결제창
    /// </summary>
    public class TossRequestVirtualAccount
    {
        /// <summary>
        /// 에스크로 사용 여부  
        /// </summary>
        public bool UseEscrow { get; set; } = false;

        /// <summary>
        /// 가상계좌 입금 만료 시간
        /// </summary>
        public int ValidHours { get; set; } = 72;
    }

    /// <summary>
    /// 결제 정보 모델
    /// 결제 시작을 위한 Request Model.
    /// Version 2
    /// </summary>
    public class TossRequestPayment
    {
        /// <summary>
        /// 결제요청시 결제 수단. 
        /// 요청은 영문 사용: CARD (카드,간편결제), VIRTUAL_ACCOUNT (가상계좌), MOBILE_PHONE (휴대폰), TRANSFER(계좌이체), 
        /// </summary>
        public TossRequestMethod Method { get; set; } = TossRequestMethod.CARD;

        /// <summary>
        /// 결제금액
        /// </summary>
        public TossAmount Amount { get; set; } = new();

        /// <summary>
        /// 필수: 주문 ID
        /// 충분히 무작위한 값을 직접 생성해서 사용하세요. 영문 대소문자, 숫자, 특수문자 -, _, =로 이루어진 6자 이상 64자 이하의 문자열이어야 합니다.
        /// </summary>
        public string OrderId { get; set; } = null!;

        /// <summary>
        /// 필수: 구매상품입니다. 예를 들면 생수 외 1건 같은 형식입니다. 최대 길이는 100자입니다.
        /// </summary>
        public string OrderName { get; set; } = null!;

        /// <summary>
        /// 구매자명입니다. 최대 길이는 100자입니다.
        /// </summary>
        public string? CustomerName { get; set; }

        /// <summary>
        /// 구매자 이메일입니다. 결제 상태가 바뀌면 이메일 주소로 결제내역이 전송됩니다. 최대 길이는 100자입니다.
        /// </summary>
        public string? CustomerEmail { get; set; }

        /// <summary>
        /// 구매자의 휴대폰 번호입니다. 가상계좌 안내, 퀵계좌이체 휴대폰 번호 자동 완성에 사용되고 있어요.
        /// </summary>
        public string? CustomerMobilePhone { get; set; }

        /// <summary>
        /// 브라우저에서 결제창이 열리는 프레임을 지정합니다. self, iframe 중 하나입니다
        /// 값을 넣지 않으면 iframe에서 결제창
        /// 현재창에서 결제창으로 이동시키는 방식을 사용하려면 값을 self로 지정하세요.
        /// * 모바일 웹에서는 windowTarget 값과 상관없이 항상 현재창에서 결제창으로 이동합니다.
        /// </summary>
        public string WindowTarget { get; set; } = "iframe";

        /// <summary>
        /// 필수: 결제가 성공하면 리다이렉트되는 URL입니다. 결제 승인 처리에 필요한 값들이 쿼리 파라미터로 함께 전달됩니다. 반드시 오리진을 포함해야 합니다. 
        /// 예를 들면 https://www.example.com/success와 같은 형태입니다.
        /// </summary>
        public string SuccessUrl { get; set; } = null!;

        /// <summary>
        /// 필수: 결제가 실패하면 리다이렉트되는 URL입니다. 에러 코드 및 에러 메시지가 쿼리 파라미터로 함께 전송됩니다. 
        /// 반드시 오리진을 포함해야 합니다.
        /// </summary>
        public string FailUrl { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TossRequestVirtualAccount? VirtualAccount { get; set; }
    }

    #endregion
}
