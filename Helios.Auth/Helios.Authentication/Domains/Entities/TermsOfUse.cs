using Helios.Authentication.Domains.Base;

namespace Helios.Authentication.Domains.Entities
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
