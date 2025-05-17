using Helios.Authentication.Domains.Base;

namespace Helios.Authentication.Domains.Entities
{
    public class TenantTermsOfUse : EntityBase
    {
        public Int64 TermsOfUseId { get; set; }
        public Int64 TenantId { get; set; }
        public TermsOfUse TermsOfUse { get; set; }
        public Tenant Tenant { get; set; }
    }
}
