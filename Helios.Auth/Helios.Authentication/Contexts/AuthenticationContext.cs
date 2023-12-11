using Helios.Authentication.Controllers.Base;
using Helios.Authentication.Entities;
using Helios.Authentication.Entities.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Helios.Authentication.Contexts
{
    public class AuthenticationContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>, ApplicationUserRole,
        IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //options.UseMySQL("Server=127.0.0.1; port=3306; Database=helios.authentication_db; Uid=root; Pwd=123Asd!!; SslMode=Preferred;");
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

        public async Task<int> SaveAuthenticationContextAsync(Guid userId, DateTimeOffset saveDate)
        {
            var transId = -1;
            if (this.ChangeTracker.HasChanges())
            {
                using (var dbContextTransaction = this.Database.BeginTransaction())
                {
                    try
                    {
                        if (this != null)
                        {
                            InsertAuthenticationBaseEntity(this, userId, saveDate);
                            UpdateAuthenticationBaseEntity(this, userId, saveDate);
                            DeleteAuthenticationBaseEntity(this, userId);
                            transId = await this.SaveChangesAsync();
                            dbContextTransaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception(ex.ToString());
                    }
                }
            }
            else
            {
                transId = 0;
            }
            return transId;
        }

        private void InsertAuthenticationBaseEntity(DbContext dbContext, Guid userId, DateTimeOffset timeStamp)
        {
            foreach (var entity in dbContext.ChangeTracker.Entries<EntityBase>().Where(x => x.State == EntityState.Added).ToList())
            {
                var dd = default(DateTimeOffset).AddDays(1);
                var dateTime = timeStamp.ToString();
                if (entity.Entity.CreatedAt < dd)
                    entity.Entity.CreatedAt = Convert.ToDateTime(dateTime);

                entity.Entity.IsDeleted = false;
                entity.Entity.IsActive = true;
                entity.Entity.AddedById = userId;
            }
        }
        private void UpdateAuthenticationBaseEntity(DbContext dbContext, Guid userId, DateTimeOffset timeStamp)
        {
            foreach (var entity in dbContext.ChangeTracker.Entries<EntityBase>().Where(x => x.State == EntityState.Modified).ToList())
            {
                var dateTime = timeStamp.ToString();
                entity.Entity.UpdatedAt = Convert.ToDateTime(dateTime);
                entity.Entity.UpdatedById = userId;
            }
        }

        private void DeleteAuthenticationBaseEntity(DbContext dbContext, Guid userId)
        {
            foreach (var entity in dbContext.ChangeTracker.Entries<EntityBase>().Where(x => x.State == EntityState.Deleted).ToList())
            {
                var dateTime = DateTimeOffset.Now.ToString();
                entity.Entity.IsDeleted = true;
                entity.Entity.IsActive = false;
                entity.Entity.UpdatedAt = Convert.ToDateTime(dateTime);
                entity.Entity.UpdatedById = userId;
                entity.State = EntityState.Modified;
            }
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantAdmin> TenantAdmins { get; set; }
        public DbSet<TermsOfUse> TermsOfUses{ get; set; }
        public DbSet<TenantTermsOfUse> TenantTermsOfUses{ get; set; }
        public DbSet<SystemAuditTrail> SystemAuditTrails { get; set; }
        public DbSet<SystemAdmin> SystemAdmins { get; set; }
    }
}
