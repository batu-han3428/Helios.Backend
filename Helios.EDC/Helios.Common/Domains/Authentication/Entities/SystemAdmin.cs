using Helios.Common.Domains.Authentication.Base;

namespace Helios.Common.Domains.Authentication.Entities
{
    public class SystemAdmin : EntityBase
    {
        public Int64 AuthUserId { get; set; }
        public ApplicationUser AuthUser { get; set; }
    }
}
