namespace Helios.Authentication.Entities.Base
{
    public interface ISecureResearch
    {
        public Guid ResearchId { get; set; }
        public Guid TenantId { get; set; }
    }
}
