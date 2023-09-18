using Helios.Core.Domains.Base;
using Helios.Core.enums;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisit : EntityBase
    {
        public Guid StudyId { get; set; }
        public Guid ReferenceKey { get; set; }
        public Guid VersionKey { get; set; }
        public VisitType VisitType{ get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool CanFreeze { get; set; }
        public bool CanLock { get; set; }
        public bool CanSign { get; set; }
        public bool CanSdv { get; set; }
        public bool CanQuery { get; set; }
        public Study Study { get; set; }
        public ICollection<StudyVisitPage> StudyVisitPages { get; set; }
    }
}
