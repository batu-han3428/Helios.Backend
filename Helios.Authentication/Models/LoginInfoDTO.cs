namespace Helios.Authentication.Models
{
    public class LoginInfoDTO
    {
        public Guid TenantId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
