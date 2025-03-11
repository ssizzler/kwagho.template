namespace kwangho.restapi.Models
{
    /// <summary>
    /// 사용자 정보
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 사용자 이름(ID)
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// 사용자 이름
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 등록일자
        /// </summary>
        public DateTimeOffset? RegisterDate { get; set; }

        /// <summary>
        /// 사용 여부
        /// </summary>
        public bool Active { get; set; } = false;
    }

    /// <summary>
    /// 사용자 정보 등록
    /// </summary>
    public class UserCreate: UserInfo
    {
        /// <summary>
        /// 비밀번호
        /// </summary>
        public string Password { get; set; } = null!;
    }

}
