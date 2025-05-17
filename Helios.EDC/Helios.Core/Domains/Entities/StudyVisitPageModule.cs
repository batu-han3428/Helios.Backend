using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPageModule : EntityBase
    {
        public StudyVisitPageModule()
        {
            StudyVisitPageModuleElements = new List<StudyVisitPageModuleElement>();
            StudyVisitPageModuleCalculationElementDetail = new List<StudyVisitPageModuleCalculationElementDetail>();
            StudyVisitPageModuleElementEvent = new List<StudyVisitPageModuleElementEvent>();
        }
        public Int64 StudyVisitPageId { get; set; }
        public string Name { get; set; }
        public Guid ReferenceKey { get; set; }
        public Int64 VersionKey { get; set; }
        public int Order { get; set; }
        public StudyVisitPage StudyVisitPage { get; set; }
        public ICollection<StudyVisitPageModuleElement> StudyVisitPageModuleElements { get; set; }
        public ICollection<StudyVisitPageModuleCalculationElementDetail> StudyVisitPageModuleCalculationElementDetail { get; set; }
        public ICollection<StudyVisitPageModuleElementEvent> StudyVisitPageModuleElementEvent { get; set; }
    }
}
