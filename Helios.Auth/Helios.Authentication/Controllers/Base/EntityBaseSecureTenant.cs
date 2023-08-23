using Helios.Authentication.Controllers.Base.Interfaces;
using Helios.Authentication.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Authentication.Controllers.Base
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
        public DateTimeOffset CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
      
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
        public DateTimeOffset CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }

    }

}
