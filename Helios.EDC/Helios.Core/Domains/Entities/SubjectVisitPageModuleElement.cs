using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class SubjectVisitPageModuleElement : EntityBase
    {
        public Guid StudyVisitPageModuleElementId { get; set; }
        public Guid SubjectVisitModuleId { get; set; }
        public string UserValue { get; set; }
        public bool ShowOnScreen { get; set; }
        public bool MissingData { get; set; }
        public bool Sdv { get; set; }
        public bool Query { get; set; }
        public StudyVisitPageModuleElement StudyVisitPageModuleElement { get; set; }
        public SubjectVisitPageModule SubjectVisitModule { get; set; }
    }
}
