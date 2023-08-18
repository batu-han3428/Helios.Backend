using Helios.eCRF.Models;

namespace Helios.eCRF.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(AccountModel model);
        Task<bool> AddTenant(TenantModel model);
        Task<bool> AddUser(UserDTO model);
        Task<bool> PassiveOrActiveUser(UserDTO model);
        Task<bool> SendNewPasswordForUser(Guid userId);
        Task<bool> UpdateUser(UserDTO model);
        Task<bool> UserProfileResetPassword(UserDTO model);

    }
}
