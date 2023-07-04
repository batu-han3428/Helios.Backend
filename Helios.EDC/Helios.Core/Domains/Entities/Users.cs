using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class Users : EntityBase
    {
        public Guid TenantId { get; set; }
        public Guid AuthUserId { get; set; }
        public int ProjectRole { get; set; }
    }
}
