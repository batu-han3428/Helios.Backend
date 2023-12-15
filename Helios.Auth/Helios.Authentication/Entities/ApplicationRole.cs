using Microsoft.AspNetCore.Identity;

namespace Helios.Authentication.Entities
{
    public class ApplicationRole : IdentityRole<Int64>
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }

    }
}
