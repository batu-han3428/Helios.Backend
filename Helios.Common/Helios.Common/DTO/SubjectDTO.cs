using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Common.DTO
{
    public class SubjectDTO
    {
        public Int64 StudyId { get; set; }
        public Int64 SiteId { get; set; }
        public Int64 Id { get; set; }
        public Int64 FirstPageId { get; set; }
        public string SubjectNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
