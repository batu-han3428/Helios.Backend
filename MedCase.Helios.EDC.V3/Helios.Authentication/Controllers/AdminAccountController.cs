using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Helpers;
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

        [HttpGet(Name = "Login")]
        public async Task<bool> Login(Guid tenantId, string username, string password)
        {
            var user = await UserManager.Users.FirstOrDefaultAsync(p => p.UserName == username && p.TenantId == tenantId);

            if (user == null) { return false; }

            var checkPassword = await UserManager.CheckPasswordAsync(user, password);

            return checkPassword;
        }


        

    }
}
