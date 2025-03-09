using kwangho.restapi.Config;
using kwangho.restapi.Context;
using kwangho.restapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    public class MemberController : ControllerBase
    {
        protected readonly ILogger _logger;

        private readonly JwtTokenIssuer _apiJwt;
        private readonly ApplicationDbContext _dbContext;

        public MemberController(ILogger<MemberController> logger, JwtTokenIssuer apiJwt, ApplicationDbContext dbContext)
        {
            _logger = logger;

            _apiJwt = apiJwt;
            _dbContext = dbContext;
        }

        #region Private Functions

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
        
        #endregion

        /// <summary>
        /// 인증 및 Access Token 발급
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("signin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiUserAcessToken>> SignIn(string userName, string userPassword)
        {
            ActionResult response = Unauthorized();
            try
            {
                var ipAddr = Request.HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString() ?? "UnKnown";
                var user = await _dbContext.ApiUser.SingleOrDefaultAsync(m => m.UserName == userName);
                var passwordHasher = new PasswordHasher<ApiUser>();
                if (user != null)
                {
                    var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, userPassword);
                    if (result == PasswordVerificationResult.Success)
                    {
                        return await GenerateJWTToken(user, ipAddr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ControllerContext.ActionDescriptor.ActionName);
                response = BadRequest();
            }
            
            return response;
        }

        /// <summary>
        /// 새로 고침 토큰 사용하여 인증 및 Access Token 발급
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("refresh-token")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiUserAcessToken>> RefreshToken(string userName, string refreshToken)
        {
            ActionResult response = Unauthorized();
            try
            {
                var ipAddr = Request.HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString() ?? "UnKnown";

                var now = DateTime.UtcNow;
                var query = from a in _dbContext.ApiUserTokenInfo
                            where a.UserName == userName && a.RefreshToken == refreshToken
                            && a.Used == false && a.Expiress > now
                            select a;
                var tokenItem = await query.FirstOrDefaultAsync();
                if (tokenItem != null)
                {
                    //새로고침 토큰이 사용되면 성공/실패 여부 상관없이 만료 한다.
                    tokenItem.Used = true;
                    await _dbContext.SaveChangesAsync();

                    var user = await _dbContext.ApiUser.SingleOrDefaultAsync(m => m.UserName == tokenItem.UserName);
                    if (user != null)
                    {
                        return await GenerateJWTToken(user, ipAddr);
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ControllerContext.ActionDescriptor.ActionName);
                response = BadRequest();
            }
            return response;
        }
    }
}
