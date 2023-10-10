using Helios.Core.Enums;

namespace Helios.Core.Services.Interfaces
{
    public interface IBaseService
    {
        void SaveSystemAuditTrail(Guid ResearchId, SystemAuditChangeType SystemAuditChangeType, string detail, string previousValues, string newValues, Guid UserId, string ClientIp);
        Task SendMail(string mail, string subject, string content);
    }
}
