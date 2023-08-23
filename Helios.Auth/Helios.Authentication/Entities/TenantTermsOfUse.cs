using Helios.Authentication.Entities.Base;

namespace Helios.Authentication.Entities
{
    public class TenantTermsOfUse : EntityBase
    {
        public Guid TermsOfUseId { get; set; }
        public Guid TenantId { get; set; }
        public TermsOfUse TermsOfUse { get; set; }
        public Tenant Tenant { get; set; }
    }
}
