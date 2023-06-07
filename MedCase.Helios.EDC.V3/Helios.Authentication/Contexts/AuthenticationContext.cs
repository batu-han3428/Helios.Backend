using Helios.Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Helios.Authentication.Contexts
{
    public class AuthenticationContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>, ApplicationUserRole,
        IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        protected readonly IConfiguration Configuration;
        public AuthenticationContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySQL("Server=127.0.0.1; port=3306; Database=helios.authentication_db; Uid=root; Pwd=123Asd!!; SslMode=Preferred;");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(m => m.Email).HasMaxLength(127);
                entity.Property(m => m.UserName).HasMaxLength(127);
            });

        }

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Study> Studies { get; set; }
    }
}
