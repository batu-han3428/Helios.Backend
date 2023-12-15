using Microsoft.AspNetCore.Identity;

namespace Helios.Authentication.Entities
{
    public class ApplicationUser : IdentityUser<Int64>
    {
        public ApplicationUser()
        {
            UserRoles = new List<ApplicationUserRole>();

        }
        public string Name { get; set; }
        public string LastName { get; set; }
        public bool ChangePassword { get; set; }
        public string PhotoBase64String { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastChangePasswordDate { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefrestTokenEndDate { get; set; }
        public bool IsResetPasswordMailSent { get; set; }
        //public string FullName => Name + " " + LastName;

        //public bool IsSso { get; set; } = false;
    }
}
