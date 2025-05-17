using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Helios.Authentication.Domains.Entities;

namespace Helios.Authentication.Domains.Base
{
    public class EntityBase : IBase
    {
        [Key]
        public Int64 Id { get; set; }
        public Int64 AddedById { get; set; }
        public Int64? UpdatedById { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }

        public ApplicationUser AddedBy { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
    }
}
