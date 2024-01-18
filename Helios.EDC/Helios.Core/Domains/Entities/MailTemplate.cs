using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class MailTemplate : EntityBase
    {
        public MailTemplate()
        {
            this.MailTemplatesRoles = new List<MailTemplatesRole>();
        }
        public string? Name { get; set; }
        public string? TemplateBody { get; set; }
        public int TemplateType { get; set; }
        public Int64 StudyId { get; set; }
        public string? ExternalMails { get; set; }
        public List<MailTemplatesRole> MailTemplatesRoles { get; set; }
    }
}
