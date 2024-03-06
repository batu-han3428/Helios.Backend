namespace Helios.Core.Domains.Entities
{
    public class MailTemplatesRole : EntityBase
    {
        public Int64 MailTemplateId { get; set; }
        public MailTemplate MailTemplate { get; set; }
        public Int64 RoleId { get; set; }
    }
}
