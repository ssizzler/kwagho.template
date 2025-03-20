using kwangho.context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace kwangho.mvc.Service
{
    /// <summary>
    /// 네비게이션 메뉴 서비스
    /// </summary>
    public class NavMenuService
    {
        private readonly ILogger _logger;
        protected readonly ApplicationDbContext _dbContext;
        private readonly IDistributedCache _cache;

        public NavMenuService(ILogger<NavMenuService> logger, ApplicationDbContext context, IDistributedCache cache)
        {
            _logger = logger;
            _dbContext = context;
            _cache = cache;
        }

        /// <summary>
        /// 메뉴 정보를 가져온다.
        /// 케시에 저장된 정보가 없으면 DB에서 가져온다.
        /// 변하지 않는 메뉴 항목을 DB에서 호출시 마다 가져오는 것은 비효율적이므로 케시를 사용한다.
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<int, List<NavMemu>>> GetMenus()
        {
            var menus = await _cache.GetAsync<Dictionary<int, List<NavMemu>>>("navMenus");
            if (menus == null)
            {
                var query = from m in _dbContext.NavMemu
                            where m.Disabled == false
                            select m;

                var items = await query
                    .AsNoTracking()
                    .ToListAsync();

                menus = items
                    .GroupBy(x => x.ParentId)
                    .ToDictionary(x => x.Key, x => x.ToList());
                await _cache.SetAsync<Dictionary<int, List<NavMemu>>>("navMenus", menus);
            }
            return menus;
        }
    }
}
