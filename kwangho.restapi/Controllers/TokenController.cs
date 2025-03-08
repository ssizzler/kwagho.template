using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kwangho.restapi.Controllers
{
    /// <summary>
    /// 인증 및 JWT 토큰을 발급하는 컨트롤러
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TokenController : ControllerBase
    {
    }
}
