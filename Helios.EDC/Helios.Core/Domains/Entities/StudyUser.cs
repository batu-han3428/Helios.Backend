using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class StudyUser : EntityBase
    {
        public Guid StudyId { get; set; }
        public Guid AuthUserId { get; set; }
        public string SuperUserIdList { get; set; }
        public Study Study { get; set; }
    }
}
