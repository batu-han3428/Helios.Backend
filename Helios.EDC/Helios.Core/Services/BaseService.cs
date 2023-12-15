using Helios.Core.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using Helios.Core.Enums;
using Helios.Core.Domains.Entities;
using Helios.Core.Services.Interfaces;

namespace Helios.Core.Services
{
    public class BaseService: IBaseService
    {
        private CoreContext _context;
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _config;
        public BaseService(CoreContext context, SmtpClient smtpClient, IConfiguration config)
        {
            _context = context;
            _smtpClient = smtpClient;
            _config = config;
        }
        public void SaveSystemAuditTrail(Int64 TenantId, SystemAuditChangeType SystemAuditChangeType, string detail, string previousValues, string newValues, Int64 UserId, string ClientIp)
        {
            //var changer = _context.Users.Where(s => s.Id == UserId).AsNoTracking().Select(a => a.Name + " " + a.LastName).FirstOrDefault();

            //var auditTrailModel = new SystemAuditTrail
            //{
            //    TenantId = TenantId,
            //    SystemAuditChangeType = SystemAuditChangeType,
            //    Details = detail,
            //    OldValue = previousValues,
            //    NewValue = newValues,
            //    UpdatedAt = DateTime.Now,
            //    ClientIp = ClientIp,
            //    Changer = !String.IsNullOrEmpty(changer) ? changer : "NA"
            //};

            //_context.SystemAuditTrails.Add(auditTrailModel);
        }

        public async Task SendMail(string mail, string subject, string content)
        {
            var mailMessage = new MailMessage(_config["EmailSender:UserName"], mail, subject, content)
            { IsBodyHtml = true, Sender = new MailAddress(_config["EmailSender:UserName"]) };

            var isSend = _smtpClient.SendMailAsync(mailMessage);
            isSend.Wait();
        }
    }
}
