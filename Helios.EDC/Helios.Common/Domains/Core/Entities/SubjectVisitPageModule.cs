using Helios.Common.Domains.Core.Base;
using Helios.Common.Enums;

namespace Helios.Common.Domains.Core.Entities
{
    public class SubjectVisitPageModule : EntityBase
    {
        public Int64 StudyVisitPageModuleId { get; set; }
        public ModuleStatus Status { get; set; }

        public StudyVisitPageModule StudyVisitPageModule { get; set; }
        public ICollection<SubjectVisitPageModuleElement> SubjectVisitPageModuleElements { get; set; }
    }
}
