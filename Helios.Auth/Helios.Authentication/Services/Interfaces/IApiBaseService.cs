namespace Helios.Authentication.Services.Interfaces
{
    public interface IApiBaseService
    {
        public Int64 UserId { get; set; }
        public Int64 StudyId { get; set; }
        public Int64 TenantId { get; set; }
    }
}
