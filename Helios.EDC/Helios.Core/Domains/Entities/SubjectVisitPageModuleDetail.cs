using Helios.Core.Domains.Base;
using Helios.Core.enums;

namespace Helios.Core.Domains.Entities
{
    public class SubjectVisitPageModuleDetail : EntityBase
    {
        public Guid SubjectVisitPageModuleId { get; set; }
        public int RowIndex { get; set; }
        public string Name { get; set; }
        public string FormNo { get; set; }
        public SubjectVisitPageModule SubjectVisitPageModule { get; set; }
    }
}
