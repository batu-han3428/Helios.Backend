using Helios.Authentication.Entities.Base;

namespace Helios.Authentication.Entities
{
    public class SystemAdmin: EntityBase
    {
        public Guid AuthUserId { get; set; }
        public ApplicationUser AuthUser { get; set; }
    }
}
