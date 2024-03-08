namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPage : EntityBase
    {
        public Int64 StudyVisitId { get; set; }
        public Int64 ReferenceKey { get; set; }
        public Int64 VersionKey { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool EPro { get; set; }
        public StudyVisit StudyVisit { get; set; }
        public ICollection<StudyVisitPageModule> StudyVisitPageModules { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
