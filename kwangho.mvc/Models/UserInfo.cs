namespace kwangho.mvc.Models
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
    /// 사용자 정보 검색
    /// </summary>
    public class UserSearch : PaginationModel
    {
        /// <summary>
        /// 검색어
        /// </summary>
        public string? SearchText { get; set; }

        /// <summary>
        /// 검색 결과 목록
        /// </summary>
        public IEnumerable<UserInfo>? Users { get; set; }

        /// <summary>
        /// 검색어 추가
        /// </summary>
        public override Dictionary<string, string> RouteData => new()
        {
            { nameof(SearchText), SearchText ?? "" }
        };
    }
}
