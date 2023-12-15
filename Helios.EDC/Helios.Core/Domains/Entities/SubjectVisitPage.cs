using Helios.Core.Domains.Base;
using Helios.Core.enums;

namespace Helios.Core.Domains.Entities
{
    public class SubjectVisitPage : EntityBase
    {
        public Int64 SubjectVisitId { get; set; }
        public Int64 StudyVisitPageId { get; set; }
        public DataStatus Status { get; set; }
        public bool Freeze { get; set; }
        public bool Lock { get; set; }
        public bool Sign { get; set; }
        public bool Sdv { get; set; }
        public bool Query { get; set; }
        public string Verification { get; set; }
        public SubjectVisit SubjectVisit { get; set; }
        public StudyVisitPage StudyVisitPage { get; set; }
        public ICollection<SubjectVisitPageModule> SubjectVisitPageModules { get; set; }
    }
}
