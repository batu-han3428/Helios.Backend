using Helios.Common.Domains.Core.Base;
using Helios.Common.Enums;

namespace Helios.Common.Domains.Core.Entities
{
    public class SubjectVisit : EntityBase
    {
        public Int64 StudyVisitId { get; set; }
        public Int64 SubjectId { get; set; }
        public Int64 ParentSubjectVisitId { get; set; }
        public Int64 RelatedSubjectVisitId { get; set; }
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
