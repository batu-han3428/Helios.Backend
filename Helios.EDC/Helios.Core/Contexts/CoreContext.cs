using Helios.Core.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Helios.Core.Contexts
{
    public class CoreContext : DbContext
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

        public async Task<int> SaveCoreContextAsync(Int64 userId, DateTimeOffset saveDate)
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
                            transId = await base.SaveChangesAsync();
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

        private void InsertAuthenticationBaseEntity(DbContext dbContext, Int64 userId, DateTimeOffset timeStamp)
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
        private void UpdateAuthenticationBaseEntity(DbContext dbContext, Int64 userId, DateTimeOffset timeStamp)
        {
            foreach (var entity in dbContext.ChangeTracker.Entries<EntityBase>().Where(x => x.State == EntityState.Modified).ToList())
            {
                var dateTime = timeStamp.ToString();
                entity.Entity.UpdatedAt = Convert.ToDateTime(dateTime);
                entity.Entity.UpdatedById = userId;
            }
        }

        private void DeleteAuthenticationBaseEntity(DbContext dbContext, Int64 userId)
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

        public DbSet<CalculatationElementDetail> CalculatationElementDetails { get; set; }
        public DbSet<ElementDetail> ElementDetails { get; set; }
        public DbSet<Element> Elements { get; set; }
        public DbSet<ElementValidationDetail> ElementValidationDetails { get; set; }
        public DbSet<MailTemplate> MailTemplates { get; set; }
        public DbSet<MailTemplateTag> MailTemplateTags { get; set; }
        public DbSet<MailTemplatesRole> MailTemplatesRoles { get; set; }
        public DbSet<ModuleElementEvent> ModuleElementEvents { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<MultipleChoiceTag> MultipleChoiceTag { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Study> Studies { get; set; }
        public DbSet<StudyRole> StudyRoles { get; set; }
        public DbSet<StudyUser> StudyUsers { get; set; }
        public DbSet<StudyUserSite> StudyUserSites { get; set; }
        public DbSet<StudyVisitPageModuleCalculationElementDetail> studyVisitPageModuleCalculationElementDetails { get; set; }
        public DbSet<StudyVisitPageModuleElementDetail> StudyVisitPageModuleElementDetails { get; set; }
        public DbSet<StudyVisitPageModuleElementEvent> StudyVisitPageModuleElementEvents { get; set; }
        public DbSet<StudyVisitPageModuleElement> StudyVisitPageModuleElements { get; set; }
        public DbSet<StudyVisitPageModuleElementValidationDetail> StudyVisitPageModuleElementValidationDetails { get; set; }
        public DbSet<StudyVisitPageModule> StudyVisitPageModules { get; set; }
        public DbSet<StudyVisitPage> StudyVisitPages { get; set; }
        public DbSet<StudyVisit> StudyVisits { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectVisitPageModuleElement> SubjectVisitPageModuleElements { get; set; }
        public DbSet<SubjectVisitPageModule> SubjectVisitPageModules { get; set; }
        public DbSet<SubjectVisitPage> SubjectVisitPages { get; set; }
        public DbSet<SubjectVisit> SubjectVisits { get; set; }
        public DbSet<SystemAuditTrail> SystemAuditTrails { get; set; }
        public DbSet<StudyVisitRelation> StudyVisitRelation {  get; set; }
    }
}
