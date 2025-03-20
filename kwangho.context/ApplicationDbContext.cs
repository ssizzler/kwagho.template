using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace kwangho.context
{
    public class ApplicationDbContext : IdentityDbContext<ApiUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        { }

        public virtual DbSet<ApiUserTokenInfo> ApiUserTokenInfo { get; set; }

        public virtual DbSet<ApiUser> ApiUser { get; set; }

        public virtual DbSet<NavMemu> NavMemu { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
