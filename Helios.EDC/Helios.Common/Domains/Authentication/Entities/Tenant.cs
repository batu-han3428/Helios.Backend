using Helios.Common.Domains.Authentication.Base;

namespace Helios.Common.Domains.Authentication.Entities
{
    public class Tenant : EntityBase
    {
        public string Name { get; set; }
        public string? StudyLimit { get; set; }
        public string? UserLimit { get; set; }
        public string? TimeZone { get; set; }
        public string? Logo { get; set; }
        List<TenantAdmin> TenantAdmin { get; set; }

    }
}
