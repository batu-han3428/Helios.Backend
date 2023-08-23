using Helios.eCRF.Enums;

namespace Helios.eCRF.Models
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Roles Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
