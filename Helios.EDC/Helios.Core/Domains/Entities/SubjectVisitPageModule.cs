using Helios.Core.Domains.Base;
using Helios.Core.enums;

namespace Helios.Core.Domains.Entities
{
    public class SubjectVisitPageModule : EntityBase
    {
        public Int64 StudyVisitPageModuleId { get; set; }
        public ModuleStatus Status { get; set; }

        public StudyVisitPageModule StudyVisitPageModule { get; set; }
        public ICollection<SubjectVisitPageModuleElement> SubjectVisitPageModuleElements { get; set; }
    }
}
