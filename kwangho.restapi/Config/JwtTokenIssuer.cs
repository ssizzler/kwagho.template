namespace kwangho.restapi.Config
{
    /// <summary>
    /// JWT 토큰 발급자 정보
    /// </summary>
    public class JwtTokenIssuer
    {
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public string Authority { get; set; } = "";

        /// <summary>
        /// JWT 토큰 만료 시간(시간)
        /// </summary>
        public int ExpiresHours { get; set; } = 1;

        /// <summary>
        /// JWT 토큰 발급자의 비밀키
        /// 이 값은 서버에서만 사용되며, 클라이언트에게 노출되어서는 안됩니다.
        /// 또한, 키값을 구성파일 등에 저장하지 말고 KeyVault 등의 안전한 저장소와 연결하여 실행시 값을 가져오도록 구성하세요.
        /// </summary>
        public string SecretKey { get; set; } = "";
    }
}
