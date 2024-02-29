using Microsoft.AspNetCore.Identity;

namespace Helios.Common.Domains.Authentication.Entities
{
    public class ApplicationRole : IdentityRole<Int64>
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }

    }
}
