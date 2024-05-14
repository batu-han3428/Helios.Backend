using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Common.Model
{
    public class ModuleModel
    {
        public Int64 Id { get; set; }
        public Int64 TenantId { get; set; }
        public Int64 UserId { get; set; }
        public Int64 AddedById { get; set; }
        public Int64 UpdatedById { get; set; }
        public string AddedNameAndLastName { get; set; }
        public string UpdatedNameAndLastName { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
