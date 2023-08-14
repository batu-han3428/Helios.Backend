﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Authentication.Entities.Base
{
    public interface IBase
    {
        Guid Id { get; set; }
        Guid AddedById { get; set; }
        Guid? UpdatedById { get; set; }
        bool IsActive { get; set; }
        bool IsDeleted { get; set; }

        [Column(TypeName = "datetime")]
        DateTimeOffset CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        DateTimeOffset UpdatedAt { get; set; }
    }
}
