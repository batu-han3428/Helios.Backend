using Helios.Common.Domains.Core.Base;
using Helios.Common.Enums;

namespace Helios.Common.Domains.Core.Entities
{
    public class StudyVisit : EntityBase
    {
        public Int64 StudyId { get; set; }
        public Int64 ReferenceKey { get; set; }
        public Int64 VersionKey { get; set; }
        public VisitType VisitType { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool CanFreeze { get; set; }
        public bool CanLock { get; set; }
        public bool CanSign { get; set; }
        public bool CanVerify { get; set; }
        public bool CanSdv { get; set; }
        public bool CanQuery { get; set; }
        public int SAELockHour { get; set; }
        public bool SAELockAction { get; set; }
        public Study Study { get; set; }
        public ICollection<StudyVisitPage> StudyVisitPages { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
