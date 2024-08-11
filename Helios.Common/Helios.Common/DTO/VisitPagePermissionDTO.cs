namespace Helios.Common.DTO
{
    public class VisitPagePermissionDTO
    {
        public Int64? StudyVisitId { get; set; }
        public Int64? StudyVisitPageId { get; set; }
        public List<int> PermissionKeys { get; set; }
    }
}
