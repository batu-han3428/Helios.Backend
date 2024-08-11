namespace Helios.Core.Domains.Entities
{
    public class StudyRole : EntityBase
    {
        public Int64 StudyId { get; set; }
        public string Name { get; set; }
        public Study Study { get; set; }
        public List<StudyUser> StudyUsers { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
