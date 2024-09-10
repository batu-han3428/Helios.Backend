using Helios.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Common.Model
{
    public class SubjectListModel
    {
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
        public SdvStatus SdvStatus { get; set; }
        public CommentType QueryStatus { get; set; }
        public int OpenQueries { get; set; }
    }
}
