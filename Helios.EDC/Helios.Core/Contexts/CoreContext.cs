using Helios.Core.Contexts.Base;
using Helios.Core.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Helios.Core.Contexts
{
    public class CoreContext : ServiceContextBase
    {
        public CoreContext(DbContextOptions<CoreContext> options) : base(options) 
        { 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet(CharSet.Utf8Mb4, true)
           .UseCollation("utf8mb4_turkish_ci");
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Site> Sites{ get; set; }
        public DbSet<Study> Studies{ get; set; }
        public DbSet<StudyRole> StudyRoles{ get; set; }
        public DbSet<StudyRoleModulePermission> StudyRoleModulePermissions{ get; set; }
        public DbSet<StudyUser> StudyUsers{ get; set; }
        public DbSet<StudyUserRole> StudyUserRoles{ get; set; }
        public DbSet<StudyUserSite> StudyUserSites{ get; set; }
        public DbSet<Element> Elements{ get; set; }
        public DbSet<ElementDetail> ElementDetails{ get; set; }
        public DbSet<Module> Modules{ get; set; }
        public DbSet<ModuleElementEvent> ModuleElementEvents{ get; set; }
        public DbSet<SystemAuditTrail> SystemAuditTrails{ get; set; }

    }
}
