using Helios.eCRF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Helios.eCRF.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(AccountModel model);
        Task<bool> SendNewPasswordForUser(Guid userId);
        Task<bool> UserProfileResetPassword(UserDTO model);
    }
}
