namespace Helios.Common.DTO
{
    public class JwtDTO
    {
        public string? Name { get; set; }
        public string?  LastName { get; set; }
        public List<string>? Roles { get; set; }
        public string? Mail { get; set; }
        public Int64? UserId { get; set; }
        public Int64? TenantId { get; set; }
        public Int64? StudyId { get; set; }
        public string? TimeZone { get; set; }
        public string ? Token { get; set; }
    }
}
