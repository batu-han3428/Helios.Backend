using Microsoft.AspNetCore.Http;

namespace Helios.Common.DTO
{
    public class TenantDTO
    {
        public Int64? Id { get; set; }
        public Int64 UserId { get; set; }
        public string TenantName { get; set; }
        public string? TimeZone { get; set; }
        public string? StudyLimit { get; set; }
        public string? UserLimit { get; set; }
        public IFormFile? TenantLogo { get; set; }
    }
}
