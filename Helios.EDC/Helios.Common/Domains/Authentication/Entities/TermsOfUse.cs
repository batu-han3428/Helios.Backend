using Helios.Common.Domains.Authentication.Base;

namespace Helios.Common.Domains.Authentication.Entities
{
    public class TermsOfUse : EntityBase
    {
        public Int64 TenantId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
        public Tenant Tenant { get; set; }
    }
}
