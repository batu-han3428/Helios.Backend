using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class MailTemplatesRoles: EntityBase
    {
        public Int64 MailTemplateId { get; set; }
        public MailTemplates MailTemplate { get; set; }
        public Int64 RoleId { get; set; }
    }
}
