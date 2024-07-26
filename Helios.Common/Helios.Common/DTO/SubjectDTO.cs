using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Common.DTO
{
    public class SubjectDTO
    {
        public Int64 StudyId { get; set; }
        public Int64 SiteId { get; set; }
        public Int64 Id { get; set; }
        public Int64 FirstPageId { get; set; }
        public string InitialName { get; set; }
        public string SubjectNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string Country { get; set; }
        public string SiteName { get; set; }
        public string RandomData { get; set; }
        public Int64 AddedById { get; set; }
        public string AddedByName { get; set; }
        public bool SDV { get; set; }
        public int Query { get; set; }

    }
    public class SubjectListModel
    {
        public List<SubjectDTO> SubjectList { get; set; }
        public bool HasSdv { get; set; }
        public bool HasQuery { get; set; }
        public bool HasRandomizasyon { get; set; }
        public bool HasRole { get; set; }
    }

    public class PermissionListModel
    {       
        public bool HasSdv { get; set; }
        public bool HasQuery { get; set; }
        public bool HasRandomizasyon { get; set; }
        public bool HasSubject { get; set; }
        public bool HasStudyDocument { get; set; }
        public bool HasMedicalCoding { get; set; }
        public bool HasIwrs { get; set; }

    }
}
