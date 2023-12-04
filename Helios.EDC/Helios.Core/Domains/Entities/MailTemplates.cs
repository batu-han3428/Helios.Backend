using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class MailTemplates : EntityBase
    {
        public MailTemplates()
        {
            this.MailTemplatesRoles = new List<MailTemplatesRoles>();
        }
        public string? Name { get; set; }
        public string? TemplateBody { get; set; }
        public int TemplateType { get; set; }
        public Guid StudyId { get; set; }
        public string? ExternalMails { get; set; }
        public List<MailTemplatesRoles> MailTemplatesRoles { get; set; }
    }
}
