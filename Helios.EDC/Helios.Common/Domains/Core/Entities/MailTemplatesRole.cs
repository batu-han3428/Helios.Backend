using Helios.Common.Domains.Core.Base;

namespace Helios.Common.Domains.Core.Entities
{
    public class MailTemplatesRole : EntityBase
    {
        public Int64 MailTemplateId { get; set; }
        public MailTemplate MailTemplate { get; set; }
        public Int64 RoleId { get; set; }
    }
}
