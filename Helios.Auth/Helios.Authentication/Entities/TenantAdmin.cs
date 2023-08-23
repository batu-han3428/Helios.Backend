using Helios.Authentication.Entities.Base;

namespace Helios.Authentication.Entities
{
    public class TenantAdmin : EntityBase
    {
        public Guid TenantId { get; set; }
        public Guid AuthUserId { get; set; }
        public Tenant Tenant { get; set; }
        public ApplicationUser AuthUser { get; set; }
    }
}
