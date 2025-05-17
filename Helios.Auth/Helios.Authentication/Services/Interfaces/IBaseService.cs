using Helios.Authentication.Enums;
using System.Net.Mail;

namespace Helios.Authentication.Services.Interfaces
{
    public interface IBaseService
    {
        void SaveSystemAuditTrail(Int64 ResearchId, SystemAuditChangeType SystemAuditChangeType, string detail, string previousValues, string newValues, Int64 UserId, string ClientIp);
        Task SendMail(string mail, string subject, string content, Attachment attachment = null);
    }
}
