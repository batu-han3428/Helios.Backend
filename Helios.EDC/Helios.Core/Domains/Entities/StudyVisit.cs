using Helios.Common.Enums;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisit : EntityBase
    {
        public StudyVisit() { Permissions = new List<Permission>(); }
        public Int64 StudyId { get; set; }
        public Int64 ReferenceKey { get; set; }
        public Int64 VersionKey { get; set; }
        public VisitType VisitType { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public Study Study { get; set; }
        public ICollection<StudyVisitPage> StudyVisitPages { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
