namespace Helios.Common.DTO
{
    public class StudyUserRolePermissionDTO
    {
        public Int64 StudyRoleId { get; set; }
        public int PermissionKey { get; set; }
    }

    public class PermissionsRoleVisitPageDTO
    {
        public Int64 StudyRoleId { get; set; }
        public Int64? StudyVisitId { get; set; }
        public Int64? StudyPageId { get; set; }
        public int PermissionKey { get; set; }
    }
}
