using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Helios.Common.Domains.Core.Base
{
    public class EntityBase : IBase
    {
        [Key]
        public Int64 Id { get; set; }
        public Int64 TenantId { get; set; }
        public Int64 AddedById { get; set; }
        public Int64? UpdatedById { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
