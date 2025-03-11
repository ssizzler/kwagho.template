using kwangho.context;
using kwangho.restapi.Config;
using kwangho.restapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.X86;
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
    public class UserController : BaseController
    {
        private readonly JwtTokenIssuer _apiJwt;

        public UserController(ILogger<UserController> logger, JwtTokenIssuer apiJwt, ApplicationDbContext dbContext)
            : base(logger, dbContext)
        {
            _apiJwt = apiJwt;
        }

        #region Private Functions

        /// <summary>
        /// Access token 생성
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<ApiUserAcessToken> GenerateJWTToken(ApiUser model)
        {
            var now = DateTimeOffset.UtcNow;
            var exp = now.AddHours(_apiJwt.ExpiresHours);  
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
                RefreshToken = await GenerateRefreshToken(model.UserName!, now),
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
        private async Task<string> GenerateRefreshToken(string userName, DateTimeOffset now)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshTokenInfo = new ApiUserTokenInfo
            {
                UserName = userName,
                Created = now.DateTime,
                RefreshToken = token,
                Expiress = now.AddDays(7).DateTime, //새로고침 토큰은 7일간 유효
                ClientIp = ClientIp ?? "UnKnown",
                Used = false
            };
            _dbContext.ApiUserTokenInfo.Add(refreshTokenInfo);

            //만료된 토큰 제거 (데이터 유지 불필요 시)
            var rmTokens = from m in _dbContext.ApiUserTokenInfo
                           where m.UserName == userName && (m.Used == true || m.Expiress < now)
                           select m;
            _dbContext.ApiUserTokenInfo.RemoveRange(rmTokens);

            await _dbContext.SaveChangesAsync();

            return token;
        }

        #endregion

        #region 토큰 발행

        /// <summary>
        /// 인증 및 Access Token 생성 발급
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
                var user = await _dbContext.ApiUser.SingleOrDefaultAsync(m => m.UserName == userName);
                var passwordHasher = new PasswordHasher<ApiUser>();
                if (user != null)
                {
                    var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, userPassword);
                    if (result == PasswordVerificationResult.Success)
                    {
                        return await GenerateJWTToken(user);
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
        /// 새로 고침 토큰 사용하여 인증 및 Access Token 생성 발급
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

                var now = DateTime.UtcNow;
                var query = from a in _dbContext.ApiUserTokenInfo
                            where a.UserName == userName && a.RefreshToken == refreshToken
                            && a.Used == false && a.Expiress > now
                            select a;
                var item = await query.FirstOrDefaultAsync();
                if (item != null)
                {
                    //새로고침 토큰이 사용되면 성공/실패 여부 상관없이 만료 한다.
                    item.Used = true;
                    await _dbContext.SaveChangesAsync();

                    var user = await _dbContext.ApiUser.SingleOrDefaultAsync(m => m.UserName == item.UserName);
                    if (user != null)
                    {
                        return await GenerateJWTToken(user);
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

        #endregion

        #region 사용자 관리

        /// <summary>
        /// 사용자 목록
        /// </summary>
        /// <returns></returns>
        [Route("list")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetUsers(string? userName = null)
        {
            ActionResult response = NoContent();

            var query = from m in _dbContext.ApiUser
                        select new UserInfo
                        {
                            UserName = m.UserName!,
                            Name = m.Name,
                            RegisterDate = m.RegisterDate,
                            Active = m.Active
                        };
            if (!string.IsNullOrWhiteSpace(userName))
                query = query.Where(m => m.UserName == userName);

            var items = await query.ToListAsync();
            if (items.Count > 0)
            {
                response = Ok(items);
            }
            return response;
        }

        /// <summary>
        /// 사용자 등록
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("create")]
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> CreateUser(UserCreate user)
        {
            var passwordHasher = new PasswordHasher<ApiUser>();
            var newUser = new ApiUser
            {
                UserName = user.UserName,
                Name = user.Name,
                RegisterDate = DateTime.Now,
                Active = true
            };
            newUser.PasswordHash = passwordHasher.HashPassword(newUser, user.Password);

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            return Created();
        }
        #endregion

    }
}
