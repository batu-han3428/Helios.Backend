using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPage : EntityBase
    {
        public Int64 StudyVisitId { get; set; }
        public Int64 ReferenceKey { get; set; }
        public Int64 VersionKey { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool CanFreeze { get; set; }
        public bool CanLock { get; set; }
        public bool CanSign { get; set; }
        public bool CanVerify { get; set; }
        public bool CanSdv { get; set; }
        public bool CanQuery { get; set; }
        public bool EPro { get; set; }
        public StudyVisit StudyVisit { get; set; }
        public ICollection<StudyVisitPageModule> StudyVisitPageModules { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
