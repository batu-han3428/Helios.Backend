namespace Helios.Common.DTO
{
    public class SSOLoginDTO
    {
        public string? Jwt { get; set; }
        public Int64 TenantId { get; set; }
        public Int64? StudyId { get; set; }
    }
}
