using Helios.Authentication.Entities.Base;

namespace Helios.Authentication.Entities
{
    public class TenantTermsOfUse : EntityBase
    {
        public Int64 TermsOfUseId { get; set; }
        public Int64 TenantId { get; set; }
        public TermsOfUse TermsOfUse { get; set; }
        public Tenant Tenant { get; set; }
    }
}
