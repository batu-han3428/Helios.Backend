using Microsoft.EntityFrameworkCore;

namespace Helios.Core.Contexts.Base
{
    public class ServiceContextBase : DbContext
    {
        protected Int64 TenantId { get; set; }
        protected Int64 CurrentUserId { get; set; }


        //protected abstract void OnModelCreating(ModelBuilder modelBuilder);
        //protected  override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.HasCharSet(CharSet.Utf8Mb4, true)
        //   .UseCollation("utf8mb4_turkish_ci");

        //    base.OnModelCreating(modelBuilder);
        //}

        public ServiceContextBase(DbContextOptions options) : base(options)
        {
        }

        public void SetCurrentUser(Int64 userId)
        {
            this.CurrentUserId = userId;
        }

        public void SetTenantId(Int64 tenantId)
        {
            this.TenantId = tenantId;
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return this.SaveChangesAsync().Result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            if (this.CurrentUserId == 0)
                throw new NullReferenceException("You must set current user of context via method name 'context.SetCurrentUser(Int64 userId)'");
            int affectedRow = 0;
            if (this.ChangeTracker.HasChanges())
            {
                using (var dbContextTransaction = this.Database.BeginTransaction())
                {
                    try
                    {
                        var dateTime = DateTime.UtcNow;
                        SetTenantBaseEntity();
                        InsertBaseEntity(this.CurrentUserId, dateTime);
                        UpdateBaseEntity(this.CurrentUserId, dateTime);
                        DeleteBaseEntity(this.CurrentUserId, dateTime);
                        affectedRow = await base.SaveChangesAsync(cancellationToken);
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        //throw new Exception(ex.ToString());
                        return -1;

                    }
                }
            }
            return affectedRow;

        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override int SaveChanges()
        {
            return this.SaveChangesAsync().Result;
        }

        private void SetTenantBaseEntity()
        {
            var tenantEntities = this.ChangeTracker.Entries<IServiceTenantBaseEntity>().Where(x => x.State != EntityState.Deleted && x.State != EntityState.Unchanged).ToList();
            if (tenantEntities.Any() && this.TenantId == 0)
                throw new Exception("TenantId cannot be null for tenant safe entitites.");

            foreach (var entity in tenantEntities)
                entity.Entity.TenantId = this.TenantId;

        }



        private void InsertBaseEntity(Int64 userId, DateTime date)
        {
            foreach (var entity in this.ChangeTracker.Entries<IServiceBaseEntity>().Where(x => x.State == EntityState.Added).ToList())
            {
                entity.Entity.CreatedAt = date;
                entity.Entity.UpdatedAt = date;
                entity.Entity.IsDeleted = entity.Entity.IsDeleted;
                entity.Entity.IsActive = entity.Entity.IsActive;
                if (entity.Entity.AddedById == 0)
                {
                    entity.Entity.AddedById = userId;
                }
            }
        }

        private void UpdateBaseEntity(Int64 userId, DateTime date)
        {
            foreach (var entity in this.ChangeTracker.Entries<IServiceBaseEntity>().Where(x => x.State == EntityState.Modified).ToList())
            {
                entity.Entity.UpdatedAt = date;
                if (entity.Entity.UpdatedById == 0 || entity.Entity.UpdatedById == null)
                {
                    entity.Entity.UpdatedById = userId;
                }
            }
        }

        private void DeleteBaseEntity(Int64 userId, DateTime date)
        {
            foreach (var entity in this.ChangeTracker.Entries<IServiceBaseEntity>().Where(x => x.State == EntityState.Deleted).ToList())
            {
                entity.Entity.IsDeleted = true;
                entity.Entity.IsActive = false;
                entity.Entity.UpdatedAt = date;
                if (entity.Entity.UpdatedById == 0 || entity.Entity.UpdatedById == null)
                {
                    entity.Entity.UpdatedById = userId;
                }
                entity.State = EntityState.Modified;
            }
        }

    }
}
