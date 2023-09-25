using Helios.Core.Domains.Base;
using Helios.Core.enums;

namespace Helios.Core.Domains.Entities
{
    public class SubjectVisit : EntityBase
    {
        public Guid StudyVisitId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid ParentSubjectVisitId { get; set; }
        public Guid RelatedSubjectVisitId { get; set; }
        public DataStatus Status { get; set; }
        public bool Freeze { get; set; }
        public bool Lock { get; set; }
        public bool Signature { get; set; }
        public bool Sdv { get; set; }
        public bool Query { get; set; }
        public bool SAELockStatus { get; set; }
        public string Verification { get; set; }
        public string FormNo { get; set; }
        public string FormName { get; set; }
        public int RowIndex { get; set; }
        public StudyVisit StudyVisit { get; set; }
        public Subject Subject { get; set; }
        public ICollection<SubjectVisitPage> SubjectVisitPages { get; set; }
    }
}
