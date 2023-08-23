using Microsoft.AspNetCore.Identity;

namespace Helios.Authentication.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }

    }
}
