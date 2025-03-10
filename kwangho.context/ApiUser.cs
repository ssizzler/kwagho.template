using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kwangho.context
{
    /// <summary>
    /// 사용자 정보
    /// </summary>
    public class ApiUser : IdentityUser
    {
        [PersonalData]
        public string? Name { get; set; }

        [PersonalData]
        public DateTimeOffset? RegisterDate { get; set; }

        public bool Active { get; set; } = false;
    }

    /// <summary>
    /// 사용자에게 발급된 토큰 정보
    /// </summary>
    [PrimaryKey("UserName", "Created")]
    public class ApiUserTokenInfo
    {
        [Key]
        public string UserName { get; set; } = string.Empty;

        [Key]
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }

        /// <summary>
        /// 새로고침 토큰
        /// </summary>
        [StringLength(1000)]
        [Unicode(false)]
        public string RefreshToken { get; set; } = null!;

        /// <summary>
        /// 만료 일자
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime Expiress { get; set; }

        /// <summary>
        /// 클라이언트 IP
        /// </summary>
        [StringLength(100)]
        [Unicode(false)]
        public string ClientIp { get; set; } = null!;

        /// <summary>
        /// 사용 여부
        /// </summary>
        public bool Used { get; set; }
    }

}
