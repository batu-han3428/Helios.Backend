using Helios.eCRF.Models;

namespace Helios.eCRF.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(AccountModel model);
        Task<bool> AddTenant(TenantModel model);

    }
}
