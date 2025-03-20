using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace kwangho.mvc.Models
{
    /// <summary>
    /// 페이지 표시 모델
    /// </summary>
    public class PaginationModel
    {
        /// <summary>
        /// 현제 페이지 번호
        /// </summary>
        /// <value></value>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// 총 아이템 수
        /// </summary>
        /// <value></value>
        public int Count { get; set; }

        /// <summary>
        /// 페이지당 표시될 아이템 수
        /// </summary>
        /// <value></value>
        public virtual int PageSize { get; set; } = 20;

        /// <summary>
        /// 페이지 시작 번호 0부터 시작
        /// </summary>
        /// <param name="1"></param>
        /// <returns></returns>
        public virtual int PageFrom => (CurrentPage - 1) * PageSize;

        /// <summary>
        /// 총 페이지 번호
        /// </summary>
        /// <returns></returns>
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        /// <summary>
        /// 페이지 번호가 표시될 최대 수
        /// </summary>
        /// <value></value>
        public int PaginationCount { get; set; } = 20;

        /// <summary>
        /// 라우트에 포함될 값 
        /// asp-all-route-data 에 설정됨
        /// </summary>
        /// <value></value>
        public virtual Dictionary<string, string> RouteData { get; set; } = new();

        /// <summary>
        /// 이동할 Action 메소드 명
        /// </summary>
        /// <value></value>
        public string? RouteAction { get; set; }

        /// <summary>
        /// 이동할 컨트롤러 명
        /// </summary>
        /// <value></value>
        public string? RouteController { get; set; }

        /// <summary>
        /// 이동할 영역 명
        /// </summary>
        public string? RouteArea { get; set; }
    }
}
