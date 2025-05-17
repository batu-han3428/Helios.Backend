namespace Helios.Core.Domains.Entities
{
    public class StudyUser : EntityBase
    {
        public Int64 StudyId { get; set; }
        public Int64 AuthUserId { get; set; }
        public string SuperUserIdList { get; set; }
        public Study Study { get; set; }
        public Int64? StudyRoleId { get; set; }
        public StudyRole StudyRole { get; set; }
        public List<StudyUserSite> StudyUserSites { get; set; }

    }
}
