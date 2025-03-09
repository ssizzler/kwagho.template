using kwangho.restapi.Filters;
using System.Text.Json.Serialization;

namespace kwangho.restapi.Models
{
    /// <summary>
    /// 사용자 AcessToken 정보
    /// </summary>
    public class ApiUserAcessToken
    {
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// AcessToken 문자
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// 만료 일자
        /// </summary>
        [JsonConverter(typeof(CustomDateTimeOffsetConverter))]
        public DateTimeOffset Expires { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// 생성 일자
        /// </summary>
        [JsonConverter(typeof(CustomDateTimeOffsetConverter))]
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// 새로고침 토큰
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// 새로고침 토큰 만료 일자
        /// </summary>
        [JsonConverter(typeof(CustomDateTimeOffsetConverter))]
        public DateTimeOffset RefreshTokenExpires { get; set; } = DateTimeOffset.UtcNow;
    }
}
