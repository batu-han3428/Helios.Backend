using Helios.Authentication.Entities.Base;

namespace Helios.Authentication.Entities
{
    public class SystemAdmin: EntityBase
    {
        public Int64 AuthUserId { get; set; }
        public ApplicationUser AuthUser { get; set; }
    }
}
