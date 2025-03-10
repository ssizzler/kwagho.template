using kwangho.tosspay.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace kwangho.tosspay
{
    public class TossPaymentService : ITossPaymentService
    {
        private readonly TossConfig _tossInfo;
        private readonly Uri _url;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="barunnConfig"></param>
        /// <param name="pgMertInfos"></param>
        /// <param name="httpClientFactory"></param>
        public TossPaymentService(IOptions<TossConfig> tossConfig, IHttpClientFactory httpClientFactory)
        {
            _url = tossConfig.Value.PaymentUrl;
            _httpClientFactory = httpClientFactory;
            _tossInfo = tossConfig.Value;
        }

        /// <summary>
        /// API 호출 시 인증 헤더 값
        /// </summary>
        private string Base64EncodedAuthenticationString
        {
            get
            {
                if (_tossInfo == null)
                    return string.Empty;
                else
                {
                    //시크릿 키 뒤에 :을 추가하고 base64로 인코딩
                    var encData_byte = Encoding.ASCII.GetBytes(_tossInfo.SecretKey + ":");
                    return Convert.ToBase64String(encData_byte);
                }
            }
        }

        /// <summary>
        /// 클라이언트키
        /// </summary>
        public string ClientKey => _tossInfo.ClientKey;

        /// <summary>
        /// 결제 승인 호출
        /// </summary>
        /// <param name="postData"></param>
        /// <returns></returns>
        public async Task<TossPayment?> ConfirmAsync(TossPostPaymentConfirm postData, string? IdempotencyKey = null)
        {
            var apiUri = new Uri(_url, "/v1/payments/confirm");
            var httpClient = _httpClientFactory.CreateClient();
            TossPayment? tossPayment = null;

            var bodystr = JsonSerializer.Serialize(postData);
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = apiUri;
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Base64EncodedAuthenticationString);
                if (!string.IsNullOrEmpty(IdempotencyKey))
                    request.Headers.Add("Idempotency-Key", IdempotencyKey);

                request.Content = new StringContent(bodystr, Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(request);

                var restr = await response.Content.ReadAsStringAsync();
                tossPayment = JsonSerializer.Deserialize<TossPayment>(restr);
            }

            return tossPayment;
        }

        /// <summary>
        /// 결제 취소
        /// 결제 취소에 성공했다면 Payment 객체의 cancels 필드에 취소 객체가 배열로 돌아옵니다.
        /// </summary>
        /// <param name="paymentKey"></param>
        /// <returns></returns>
        public async Task<TossPayment?> CancelAsync(string paymentKey, TossPostPaymentCancel postData, string? IdempotencyKey = null)
        {
            var apiUri = new Uri(_url, $"/v1/payments/{paymentKey}/cancel");
            var httpClient = _httpClientFactory.CreateClient();
            TossPayment? tossPayment = null;

            var bodystr = JsonSerializer.Serialize(postData);
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = apiUri;
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Base64EncodedAuthenticationString);
                if (!string.IsNullOrEmpty(IdempotencyKey))
                    request.Headers.Add("Idempotency-Key", IdempotencyKey);

                request.Content = new StringContent(bodystr, Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var restr = await response.Content.ReadAsStringAsync();
                tossPayment = JsonSerializer.Deserialize<TossPayment>(restr);
            }

            return tossPayment;
        }
    }
}
