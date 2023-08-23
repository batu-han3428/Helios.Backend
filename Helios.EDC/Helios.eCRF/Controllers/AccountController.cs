using Helios.eCRF.Controllers.Base;
using Helios.eCRF.Models;
using Helios.eCRF.Services;
using Helios.eCRF.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Mail;

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
    }
}
