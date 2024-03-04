using Helios.Common.Domains.Core.Base;

namespace Helios.Common.Domains.Core.Entities
{
    public class MailTemplateTag : EntityBase
    {
        public string? Tag { get; set; }
        public int TemplateType { get; set; }
    }
}
