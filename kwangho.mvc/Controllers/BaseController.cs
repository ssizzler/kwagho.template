using kwangho.context;
using Microsoft.AspNetCore.Mvc;

namespace kwangho.mvc.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILogger _logger;
        protected readonly ApplicationDbContext _dbContext;

        public BaseController(ILogger logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 접속 클라이언트 IP 주소
        /// </summary>
        protected string? ClientIp
        {
            get
            {
                var remoteIp = Request.HttpContext.Connection.RemoteIpAddress;
                if (remoteIp != null && remoteIp.IsIPv4MappedToIPv6)
                    return remoteIp.MapToIPv4().ToString();
                else
                    return remoteIp?.ToString();
            }

        }
    }
}
