using Helios.Authentication.Enums;

namespace Helios.Authentication.Services.Interfaces
{
    public interface IBaseService
    {
        void SaveSystemAuditTrail(Guid ResearchId, SystemAuditChangeType SystemAuditChangeType, string detail, string previousValues, string newValues, Guid UserId, string ClientIp);
    }
}
