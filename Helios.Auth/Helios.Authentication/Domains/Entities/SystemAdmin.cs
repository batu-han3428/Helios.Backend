using Helios.Authentication.Domains.Base;

namespace Helios.Authentication.Domains.Entities
{
    public class SystemAdmin : EntityBase
    {
        public Int64 AuthUserId { get; set; }
        public ApplicationUser AuthUser { get; set; }
    }
}
