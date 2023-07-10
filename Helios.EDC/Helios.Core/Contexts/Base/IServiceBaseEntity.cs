using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Core.Contexts.Base
{
    public interface IServiceBaseEntity
    {
        Guid Id { get; set; }
        Guid AddedById { get; set; }
        Guid? UpdatedById { get; set; }
        bool IsActive { get; set; }
        bool IsDeleted { get; set; }

        [Column(TypeName = "datetime")]
        DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        DateTime UpdatedAt { get; set; }


    }
    public interface IServiceTenantBaseEntity
    {
        Guid TenantId { get; set; }
    }
}
