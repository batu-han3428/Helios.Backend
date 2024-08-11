namespace Helios.Core.Domains.Entities
{
    public class StudyUserSite : EntityBase
    {
        public Int64 StudyUserId { get; set; }
        public Int64 SiteId { get; set; }
        public StudyUser StudyUser { get; set; }
        public Site Site { get; set; }
    }
}
