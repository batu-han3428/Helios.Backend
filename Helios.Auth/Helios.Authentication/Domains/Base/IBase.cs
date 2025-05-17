using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Authentication.Domains.Base
{
    public interface IBase
    {
        Int64 Id { get; set; }
        Int64 AddedById { get; set; }
        Int64? UpdatedById { get; set; }
        bool IsActive { get; set; }
        bool IsDeleted { get; set; }

        [Column(TypeName = "datetime")]
        DateTimeOffset CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        DateTimeOffset UpdatedAt { get; set; }
    }
}
