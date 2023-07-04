using Helios.eCRF.Services.Base;
using System.Xml.Linq;

namespace Helios.eCRF.Services.Interfaces
{
    public interface IBaseService : IApiBaseService, IClientTimeZoneOffset
    {
        IQueryable<T> GetResearchDBListPaging<T>() where T : EntityBaseSecureTenant;
        IQueryable<T> GetResearchDBListPaging<T>(string orderCriteria, bool orderAscendingDirection, out int total, int start, int length) where T : EntityBaseSecureTenant;

        IQueryable<T> GetEntityQueryableInResearchDatabase<T>() where T : EntityBaseSecureTenant;
        IQueryable<T> GetEntityQueryableInAppDatabase<T>() where T : class;
        Guid GetTenantId(Guid researchId);
    }


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
