using Helios.eCRF.Models;
using Helios.eCRF.Services;
using Helios.eCRF.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

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
        public async Task<IActionResult> Login([FromForm] AccountModel user)
        {
            var result = await authService.LoginAsync(user);

            return Ok("Form data received successfully");
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

    }
}
