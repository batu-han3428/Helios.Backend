namespace Helios.Core.Domains.Base
{
    public interface ISecureResearch
    {
        public Guid ResearchId { get; set; }
        public Guid TenantId { get; set; }
    }
}
