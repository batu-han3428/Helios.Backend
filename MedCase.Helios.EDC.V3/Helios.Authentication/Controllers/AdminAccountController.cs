using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Helpers;
using Helios.Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helios.Authentication.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminAccountController : ControllerBase
    {
        private readonly AuthenticationContext _context;
        UserManager<ApplicationUser> UserManager { get; set; }

        public AdminAccountController(AuthenticationContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<bool> Login(LoginInfoDTO model)
        {
            var user = await UserManager.Users.FirstOrDefaultAsync(p => p.UserName == model.Username && p.TenantId == model.TenantId);

            if (user == null) { return false; }

            var checkPassword = await UserManager.CheckPasswordAsync(user, model.Password);

            return checkPassword;
        }


        

    }
}
