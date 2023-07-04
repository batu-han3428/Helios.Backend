﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.eCRF.Services.Interfaces
{
    public interface IBase
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
}
