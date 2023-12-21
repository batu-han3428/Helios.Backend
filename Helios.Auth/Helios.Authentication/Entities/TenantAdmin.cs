using Helios.Authentication.Entities.Base;

namespace Helios.Authentication.Entities
{
    public class TenantAdmin : EntityBase
    {
        public Int64 AuthUserId { get; set; }
        public List<TenantsTenantAdmins> TenantsTenantAdmins { get; set; }
        public ApplicationUser AuthUser { get; set; }
    }
}
