using Helios.Common.Enums;

namespace Helios.Core.Services.Interfaces
{
    public interface IBaseService
    {
        void SaveSystemAuditTrail(Int64 TenantId, SystemAuditChangeType SystemAuditChangeType, string detail, string previousValues, string newValues, Int64 UserId, string ClientIp);
        Task SendMail(string mail, string subject, string content);
    }
}
