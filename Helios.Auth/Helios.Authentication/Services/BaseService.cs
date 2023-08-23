using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Enums;
using Helios.Authentication.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Helios.Authentication.Services
{
    public class BaseService : IBaseService
    {
        private AuthenticationContext _context;

        protected BaseService(AuthenticationContext context)
        {
            _context = context;
        }

        public void SaveSystemAuditTrail(Guid TenantId, SystemAuditChangeType SystemAuditChangeType, string detail, string previousValues, string newValues, Guid UserId, string ClientIp)
        {
            var changer = _context.Users.Where(s => s.Id == UserId).AsNoTracking().Select(a => a.Name + " " + a.LastName).FirstOrDefault();

            var auditTrailModel = new SystemAuditTrail
            {
                TenantId = TenantId,
                SystemAuditChangeType = SystemAuditChangeType,
                Details = detail,
                OldValue = previousValues,
                NewValue = newValues,
                UpdatedAt = DateTime.Now,
                ClientIp = ClientIp,
                Changer = !String.IsNullOrEmpty(changer) ? changer : "NA"
            };

            _context.SystemAuditTrails.Add(auditTrailModel);
        }
    }
}
