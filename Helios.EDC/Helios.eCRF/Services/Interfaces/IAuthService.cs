using Helios.eCRF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Mail;

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
        Task<List<TenantModel>> GetTenantList();
        Task ContactUsMailAsync(ContactUsDTO contactUsDTO);
        Task<dynamic> SaveForgotPassword(string Mail);
        Task<dynamic> ResetPasswordGet(string code, string username, bool firstPassword);
        Task<dynamic> ResetPasswordPost(ResetPasswordViewModel model);
    }
}
