using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Common.Model
{
    public class VisitPageModuleModel
    {
        public Int64 Id { get; set; }
        public string ModuleName { get; set; }
        public int Order { get; set; }

        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
