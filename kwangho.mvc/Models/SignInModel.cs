using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace kwangho.mvc.Models
{
    /// <summary>
    /// 로그인 모델
    /// </summary>
    public class SignInModel
    {
        [Required(ErrorMessage = "아이디를 입력해주세요.")]
        [Display(Name = "아이디", Prompt = "아이디를 입력해주세요")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 비번
        /// </summary>
        [Required(ErrorMessage = "비밀번호를 입력해주세요")]
        [DataType(DataType.Password)]
        [Display(Name = "비밀번호", Prompt = "비밀번호를 입력해주세요")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 이동 경로
        /// </summary>
        [ValidateNever]
        public string ReturnUrl { get; set; } = "/";
    }
}
