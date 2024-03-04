using Helios.Common.Domains.Authentication.Base;

namespace Helios.Common.Domains.Authentication.Entities
{
    public class TenantAdmin : EntityBase
    {
        public Int64 TenantId { get; set; }
        public Int64 AuthUserId { get; set; }
        public Tenant Tenant { get; set; }
        public ApplicationUser AuthUser { get; set; }
    }
}
