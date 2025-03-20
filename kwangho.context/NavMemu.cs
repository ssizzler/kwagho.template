using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kwangho.context
{
    /// <summary>
    /// 네비게이션 메뉴
    /// </summary>
    public class NavMemu
    {
        /// <summary>
        /// 메뉴 아이디
        /// </summary>
        [Key] 
        public int Id { get; set; }

        /// <summary>
        /// 이동할 Action 메소드 명
        /// </summary>
        [StringLength(100)] 
        public string? ActionName { get; set; }

        /// <summary>
        /// 이동할 Controller 명
        /// </summary>
        [StringLength(100)] 
        public string? ControllerName { get; set; }

        /// <summary>
        /// Area 명
        /// </summary>
        [StringLength(100)] 
        public string? AreaName { get; set; }

        /// <summary>
        /// 이동할 URL
        /// 우선순위가 가장 높음
        /// </summary>
        [StringLength(1000)] 
        public string? UrlString { get; set; }

        /// <summary>
        /// 메뉴 타이틀
        /// </summary>
        [StringLength(100)] 
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 상위 메뉴 아이디
        /// 최상위 메뉴는 0
        /// </summary>
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// 메뉴 활성화 여부
        /// </summary>
        public bool Disabled { get; set; } = false;

        /// <summary>
        /// 등록일
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }

        /// <summary>
        /// 정렬 순서
        /// </summary>
        public int SortOrder { get; set; } = 1;
    }
}
