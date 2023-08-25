using Microsoft.AspNetCore.Identity;

namespace Helios.Authentication.Entities
{
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        public override Guid UserId { get; set; }
        public override Guid RoleId { get; set; }
        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }
        public Guid TenantId { get; set; }
    }
}
