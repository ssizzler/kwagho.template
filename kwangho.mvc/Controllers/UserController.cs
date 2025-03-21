using kwangho.context;
using kwangho.mvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace kwangho.mvc.Controllers
{
    public class UserController : BaseController
    {
        public UserController(ILogger<UserController> logger, ApplicationDbContext dbContext) : base(logger, dbContext)
        { }

        /// <summary>
        /// 로그인 화면
        /// </summary>
        /// <param name="returnUrl">성공시 이동 Url</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SignIn(string? returnUrl = null)
        {
            var model = new SignInModel
            {
                ReturnUrl = returnUrl ?? Url.Action("Index", "Home")!
            };

            return View(model);
        }

        /// <summary>
        /// 로그인 처리
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _dbContext.ApiUser.SingleOrDefaultAsync(m => m.UserName == model.UserId);
                var passwordHasher = new PasswordHasher<ApiUser>();
                if (user != null)
                {
                    var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, model.Password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        var claims = new List<Claim>
                        {
                            new (ClaimTypes.Sid, user.UserName!),
                            new (ClaimTypes.Name, user.UserName!)
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));

                        return LocalRedirect(model.ReturnUrl);
                    }
                }
                ModelState.AddModelError(string.Empty, "아이디 또는 비밀번호가 일치하지 않습니다.");
            }
            return View(model);
            
        }

        /// <summary>
        /// 로그아웃
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public new async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 사용자 목록
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Users(UserSearch model)
        {
            model.RouteController = ControllerContext.ActionDescriptor.ControllerName;
            model.RouteAction = ControllerContext.ActionDescriptor.ActionName;

            try
            {
                var query = from m in _dbContext.ApiUser
                            where m.Active == true
                            && (string.IsNullOrEmpty(model.SearchText) || (m.Name != null && m.Name.StartsWith(model.SearchText)) || (m.UserName != null && m.UserName.StartsWith(model.SearchText)))
                            orderby m.UserName ascending
                            select new UserInfo
                            {
                                UserName = m.UserName!,
                                Name = m.Name,
                                RegisterDate = m.RegisterDate,
                                Active = m.Active
                            };
                model.Count = await query.CountAsync();

                //페이지 수 만큼 데이터 읽기
                query = query.Skip(model.PageFrom).Take(model.PageSize);
                model.Users = await query.ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ControllerContext.ActionDescriptor.ActionName);
            }
            return View(model);
        }
    }
}
