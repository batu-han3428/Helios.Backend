using Helios.eCRF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Mail;

namespace Helios.eCRF.Services.Interfaces
{
    public interface IAuthService
    {
        Task<dynamic> LoginAsync(AccountModel model);
        Task<bool> SendNewPasswordForUser(Guid userId);
        Task<bool> UserProfileResetPassword(UserDTO model);
        Task ContactUsMailAsync(ContactUsDTO contactUsDTO);
        Task<dynamic> SaveForgotPassword(string Mail);
        Task<dynamic> ResetPasswordGet(string code, string username, bool firstPassword);
        Task<dynamic> ResetPasswordPost(ResetPasswordViewModel model);
    }
}
