using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class MailTemplatesRoles: EntityBase
    {
        public Guid MailTemplateId { get; set; }
        public MailTemplates MailTemplate { get; set; }
        public Guid RoleId { get; set; }
    }
}
