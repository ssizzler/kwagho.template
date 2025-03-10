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

    /// <summary>
    /// 결제 요청에 실패 Response Model
    /// https://docs.tosspayments.com/reference/error-codes#failurl%EB%A1%9C-%EC%A0%84%EB%8B%AC%EB%90%98%EB%8A%94-%EC%97%90%EB%9F%AC
    /// JavaScript Client
    /// </summary>
    public class TossCallBackPaymentFail : TossResponseMessage
    {
        [JsonPropertyName("orderId")]
        public string? OrderId { get; set; }
    }

    #endregion

}
