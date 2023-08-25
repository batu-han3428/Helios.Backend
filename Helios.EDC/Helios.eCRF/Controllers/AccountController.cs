using Helios.eCRF.Models;
using Helios.eCRF.Services;
using Helios.eCRF.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Helios.eCRF.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IUserService userService;

        public AccountController(IAuthService authService, IUserService userService)
        {
            this.authService = authService;
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountModel user)
        {
            var result = await authService.LoginAsync(user);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> SendNewPasswordForUser(Guid userId)
        {
            var result = await authService.SendNewPasswordForUser(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UserProfileResetPassword(UserDTO model)
        {
            var result = await userService.UpdateUser(model);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ContactUs(ContactUsDTO contactUsModel)
        {
            try
            {
                await authService.ContactUsMailAsync(contactUsModel);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return NotFound(false);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveForgotPassword([FromBody] string Mail)
        {   
            if (string.IsNullOrEmpty(Mail))
            {
                return NotFound(new { isSuccess= false, message = "Mail alanı boş bırakılamaz" });
            }

            var result = await authService.SaveForgotPassword(Mail);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string code = null, string username = null, bool firstPassword = false)
        {
            if (code == null || username == null)
            {
                return Ok(new { isSuccess = false, messsage = "Şifre sıfırlama için kod, kullanıcı adı ve çalışma adı sağlanmalıdır." });
            }

            var result = await authService.ResetPasswordGet(code, username, firstPassword);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(model);
            }

            var result = await authService.ResetPasswordPost(model);

            return Ok(result);
        }
    }
}
