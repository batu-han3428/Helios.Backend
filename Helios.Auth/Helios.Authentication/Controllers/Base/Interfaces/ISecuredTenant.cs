using System;

namespace Helios.Authentication.Controllers.Base.Interfaces
{
    public interface ISecuredTenant
    {
        Guid TenantId { get; set; }
    }
}
