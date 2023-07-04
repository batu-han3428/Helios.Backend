﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Helios.eCRF.Services.Interfaces;

namespace Helios.eCRF.Services.Base
{
    public abstract class EntityBaseSecureTenant : ISecuredTenant, IBase
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AddedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid TenantId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

    }

    public abstract class EntityBaseNonTenant : IBase
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AddedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

    }
}
