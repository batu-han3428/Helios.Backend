using Helios.Common.Domains.Core.Base;
using Helios.Common.Enums;

namespace Helios.Common.Domains.Core.Entities
{
    public class SystemAuditTrail : EntityBase
    {
        public Int64 TenantId { get; set; }
        public string Changer { get; set; }
        public string ClientIp { get; set; }
        public SystemAuditChangeType SystemAuditChangeType { get; set; }
        public string Details { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
    }
}
