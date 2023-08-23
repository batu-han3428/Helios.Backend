using Helios.eCRF.Controllers.Base;
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

        public AccountController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] AccountModel user)
        {
            var result = await authService.LoginAsync(user);

            return Ok("Form data received successfully");
        }

        [HttpPost]
        public async Task<bool> AddTenant(string name)
        {
            var model = new TenantModel { Name = name };
            var result = await authService.AddTenant(model);
            return result;
            //return Ok("Form data received successfully"); 
        }
    
        [HttpGet]
        public async Task<List<TenantModel>> GetTenantList()
        {
            var result = await authService.GetTenantList();

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser(UserDTO model)
        {
            var result = await authService.AddUser(model);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> PassiveOrActiveUser(UserDTO model)
        {
            var result = await authService.PassiveOrActiveUser(model);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> SendNewPasswordForUser(Guid userId)
        {
            var result = await authService.SendNewPasswordForUser(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserDTO model)
        {
            var result = await authService.UpdateUser(model);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UserProfileResetPassword(UserDTO model)
        {
            var result = await authService.UpdateUser(model);
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
