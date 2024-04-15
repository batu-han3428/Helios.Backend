namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPage : EntityBase
    {
        public StudyVisitPage() { Permissions = new List<Permission>(); }
        public Int64 StudyVisitId { get; set; }
        public Guid ReferenceKey { get; set; }
        public Int64 VersionKey { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool EPro { get; set; }
        public StudyVisit StudyVisit { get; set; }
        public ICollection<StudyVisitPageModule> StudyVisitPageModules { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
