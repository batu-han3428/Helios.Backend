using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Core.Contexts.Base
{
    public interface IServiceBaseEntity
    {
        Int64 Id { get; set; }
        Int64 AddedById { get; set; }
        Int64? UpdatedById { get; set; }
        bool IsActive { get; set; }
        bool IsDeleted { get; set; }

        [Column(TypeName = "datetime")]
        DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        DateTime UpdatedAt { get; set; }


    }
    public interface IServiceTenantBaseEntity
    {
        Int64 TenantId { get; set; }
    }
}
