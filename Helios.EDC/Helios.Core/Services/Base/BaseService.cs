using Helios.Core.Contexts;
using System.Net.Mail;
using Helios.Common.Enums;
using Helios.Core.Services.Interfaces;

namespace Helios.Core.Services.Base
{
    public class BaseService : IBaseService
    {
        private CoreContext _context;
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _config;
        public BaseService(CoreContext context, /*SmtpClient smtpClient,*/ IConfiguration config)
        {
            _context = context;
            //_smtpClient = smtpClient;
            _config = config;
        }
        public void SaveSystemAuditTrail(long TenantId, SystemAuditChangeType SystemAuditChangeType, string detail, string previousValues, string newValues, long UserId, string ClientIp)
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
            //var mailMessage = new MailMessage(_config["EmailSender:UserName"], mail, subject, content)
            //{ IsBodyHtml = true, Sender = new MailAddress(_config["EmailSender:UserName"]) };

            //var isSend = _smtpClient.SendMailAsync(mailMessage);
            //isSend.Wait();
        }
    }
}
