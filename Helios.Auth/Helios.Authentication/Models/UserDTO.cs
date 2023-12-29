using Helios.Common.Enums;

namespace Helios.Authentication.Models
{
    public class UserDTO
    {
        public Int64 TenantId { get; set; }
        public Int64 StudyId { get; set; }
        public Roles Role{ get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Int64 UserId { get; set; }
        public string ConfirmPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
