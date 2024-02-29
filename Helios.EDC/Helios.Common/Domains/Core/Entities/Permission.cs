using Helios.Common.Domains.Core.Base;

namespace Helios.Common.Domains.Core.Entities
{
    public class Permission : EntityBase
    {
        public Int64? StudyRoleId { get; set; }
        public StudyRole StudyRole { get; set; }
        public Int64? StudyVisitId { get; set; }
        public StudyVisit StudyVisit { get; set; }
        public Int64? StudyVisitPageId { get; set; }
        public StudyVisitPage StudyVisitPage { get; set; }
        public int PermissionName { get; set; }
        public Int64 StudyId { get; set; }
    }
}
