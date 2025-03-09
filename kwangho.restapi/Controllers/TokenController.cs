using kwangho.restapi.Config;
using kwangho.restapi.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
        private readonly JwtTokenIssuer _apiJwt;
        private readonly ApplicationDbContext _dbContext;
        private readonly SignInManager<ApiUser> _signInManager;
        public TokenController(JwtTokenIssuer apiJwt, ApplicationDbContext dbContext, SignInManager<ApiUser> signInManager)
        {
            _apiJwt = apiJwt;
            _dbContext = dbContext;
            _signInManager = signInManager;
        }

        #region Private Functions

        /// <summary>
        /// 인증 확인
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private async Task<ApiUser?> AuthenticateClient(string userName, string userPassword)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, userPassword, false, lockoutOnFailure: false);
            if (result.Succeeded)
                return await _dbContext.Users.FindAsync(userName);
            else
                return null;
        }

        /// <summary>
        /// 새로고침 토큰 생성
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientIp"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        private async Task<string> GenerateRefreshToken(string userName, string clientIp, DateTimeOffset now)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshTokenInfo = new ApiUserTokenInfo
            {
                UserName = userName,
                Created = now.DateTime,
                RefreshToken = token,
                Expiress = now.AddDays(7).DateTime,
                ClientIp = clientIp,
                Used = false
            };
            _dbContext.ApiUserTokenInfo.Add(refreshTokenInfo);

            //만료된 토큰 제거 (데이터 유지 불필요)
            var rmTokens = from m in _dbContext.ApiUserTokenInfo
                           where m.UserName == userName && (m.Used == true || m.Expiress < now)
                           select m;
            _dbContext.ApiUserTokenInfo.RemoveRange(rmTokens);

            await _dbContext.SaveChangesAsync();

            return token;
        }

        /// <summary>
        /// Access token 생성
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<ApiUserAcessToken> GenerateJWTToken(ApiUser model, string clientIp)
        {
            var now = DateTimeOffset.UtcNow;
            var exp = now.AddHours(1);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiJwt.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sid, model.UserName!),
                new Claim(JwtRegisteredClaimNames.Name, model.Name!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _apiJwt.Issuer,
                audience: _apiJwt.Audience,
                claims: claims,
                expires: exp.LocalDateTime,
                signingCredentials: credentials
            );

            return new ApiUserAcessToken
            {
                UserName = model.UserName!,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = exp,
                Created = now,
                RefreshToken = await GenerateRefreshToken(model.UserName!, clientIp, now),
                RefreshTokenExpires = now.AddDays(7)
            };
        }
        #endregion

        /// <summary>
        /// 인증 및 Access Token 발급
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("authenticate")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiUserAcessToken>> Authenticate(string userName, string userPassword)
        {
            ActionResult response = Unauthorized();
            try
            {
                var ipAddr = Request.HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString() ?? "UnKnown";
                var user = await AuthenticateClient(userName, userPassword);
                if (user != null)
                {
                    return await GenerateJWTToken(user, ipAddr);
                }
            }
            catch
            {
                response = BadRequest();
            }
            return response;
        }

    }
}
