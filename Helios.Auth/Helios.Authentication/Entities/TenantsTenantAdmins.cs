using Helios.Authentication.Entities.Base;

namespace Helios.Authentication.Entities
{
    public class TenantsTenantAdmins : EntityBase
    {
        public Int64 TenantAdminId { get; set; }
        public TenantAdmin TenantAdmin { get; set; }
        public Int64 TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }
}
