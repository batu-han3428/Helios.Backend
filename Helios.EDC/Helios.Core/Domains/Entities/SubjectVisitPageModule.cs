using Helios.Common.Enums;

namespace Helios.Core.Domains.Entities
{
    public class SubjectVisitPageModule : EntityBase
    {
        public Int64 StudyVisitPageModuleId { get; set; }
        public ModuleStatus Status { get; set; }

        public StudyVisitPageModule StudyVisitPageModule { get; set; }
        public SubjectVisitPage SubjectVisitPage { get; set; }
        public ICollection<SubjectVisitPageModuleElement> SubjectVisitPageModuleElements { get; set; }
    }
}
