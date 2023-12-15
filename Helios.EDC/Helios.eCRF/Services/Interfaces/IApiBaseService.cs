namespace Helios.eCRF.Services.Interfaces
{
    public interface IApiBaseService
    {
        string ClientIp { get; set; }
        string HttpContextRequestPath { get; set; }
        Int64 UserId { get; set; }
        Int64 ResearchId { get; set; }
        Int64 TenantId { get; set; }
        Int64 ResearcherId { get; set; }
    }
}
