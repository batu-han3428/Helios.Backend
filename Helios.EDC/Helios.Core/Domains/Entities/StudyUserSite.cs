using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class StudyUserSite : EntityBase
    {
        public Guid StudyUserId { get; set; }
        public Guid SiteId { get; set; }
        public StudyUser StudyUser{ get; set; }
        public Site Site { get; set; }
    }
}
