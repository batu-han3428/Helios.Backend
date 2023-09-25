using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPage : EntityBase
    {
        public Guid StudyVisitId { get; set; }
        public Guid ReferenceKey { get; set; }
        public Guid VersionKey { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool CanFreeze { get; set; }
        public bool CanLock { get; set; }
        public bool CanSign { get; set; }
        public bool CanVerify { get; set; }
        public bool CanSdv { get; set; }
        public bool CanQuery { get; set; }
        public StudyVisit StudyVisit { get; set; }
        public ICollection<StudyVisitPageModule> StudyVisitPageModules { get; set; }
    }
}
