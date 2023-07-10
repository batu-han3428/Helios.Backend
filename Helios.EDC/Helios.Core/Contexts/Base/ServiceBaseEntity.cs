using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Helios.Core.Contexts.Base
{
    public abstract class ServiceBaseEntity : IServiceBaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AddedById { get; set; }
        public Guid? UpdatedById { get; set; }


        [Column(TypeName = "bit(1)")]
        public bool IsActive { get; set; } = true;
        [Column(TypeName = "bit(1)")]
        public bool IsDeleted { get; set; } = false;

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }



    }


    public abstract class TenantServiceBaseEntity : ServiceBaseEntity, IServiceTenantBaseEntity
    {

        /// <summary>
        /// Study spesific Id => ResearchId or TenantId
        /// </summary>
        public Guid TenantId { get; set; }
    }
}
