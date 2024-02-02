using Helios.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Common.Model
{
    public class VisitModel
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public VisitType VisitType { get; set; }
        public bool? EPro { get; set; }
        public int Order { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
        public List<VisitModel> Children { get; set; }
    }
}
