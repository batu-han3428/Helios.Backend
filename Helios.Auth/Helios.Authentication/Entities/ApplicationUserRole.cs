using Microsoft.AspNetCore.Identity;

namespace Helios.Authentication.Entities
{
    public class ApplicationUserRole : IdentityUserRole<Int64>
    {
        public override Int64 UserId { get; set; }
        public override Int64 RoleId { get; set; }
        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }
        public Int64 TenantId { get; set; }
        public Int64 StudyId { get; set; }
    }
}
