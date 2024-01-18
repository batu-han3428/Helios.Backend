using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class MailTemplateTag : EntityBase
    {
        public string? Tag { get; set; }
        public int TemplateType { get; set; }
    }
}
