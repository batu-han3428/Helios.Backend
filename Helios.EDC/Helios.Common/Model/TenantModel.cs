using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Common.Model
{
    public class TenantModel
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string? StudyLimit { get; set; }
        public string? ActiveStudies { get; set; }
        public string? TimeZone { get; set; }
        public string? UserLimit { get; set; }
        public string Logo { get; set; }
        public string? Path { get; set; }

        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
