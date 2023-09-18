using Helios.Core.Domains.Base;
using Helios.Core.enums;

namespace Helios.Core.Domains.Entities
{
    public class SubjectVisitPageModule : EntityBase
    {
        public Guid SubjectVisitId { get; set; }
        public Guid StudyVisitPageModuleId { get; set; }
        public ModuleStatus Status { get; set; }

        public SubjectVisit SubjectVisit { get; set; }
        public StudyVisitPageModule StudyVisitPageModule { get; set; }
        public ICollection<SubjectVisitPageModuleElement> SubjectVisitPageModuleElements { get; set; }
    }
}
