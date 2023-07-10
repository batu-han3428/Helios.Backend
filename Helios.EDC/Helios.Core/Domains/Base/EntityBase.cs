﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Helios.Core.Domains.Base
{
    public class EntityBase : IBase
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid AddedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

    }
}
