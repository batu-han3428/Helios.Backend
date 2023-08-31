using Helios.Authentication.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Authentication.Entities
{
    public class Tenant
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? AddedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTimeOffset? UpdatedAt { get; set; }
        public string Name { get; set; }
    }
}
