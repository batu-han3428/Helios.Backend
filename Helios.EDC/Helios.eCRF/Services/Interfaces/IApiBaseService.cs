namespace Helios.eCRF.Services.Interfaces
{
    public interface IApiBaseService
    {
        string ClientIp { get; set; }
        string HttpContextRequestPath { get; set; }
        Guid UserId { get; set; }
        Guid ResearchId { get; set; }
        Guid TenantId { get; set; }
        Guid ResearcherId { get; set; }
    }
}
