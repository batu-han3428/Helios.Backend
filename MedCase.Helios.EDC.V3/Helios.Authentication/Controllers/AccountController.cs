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
    public class AccountController : Controller
    {
        private readonly AuthenticationContext _context;
        UserManager<ApplicationUser> UserManager { get; set; }

        public AccountController(AuthenticationContext context)
        {
            _context = context;
        }
        //public async Task<bool> AddRole(Guid tenantId, string firstName, string lastName, string email)
        //{
        //    var result = false;

        //    return result;
        //}

        [HttpPost]
        public async Task<bool> Login(LoginInfoDTO model)
        {
            var user = await UserManager.Users.FirstOrDefaultAsync(p => p.UserName == model.Username && p.TenantId == model.TenantId);

            if (user == null) { return false; }

            var checkPassword = await UserManager.CheckPasswordAsync(user, model.Password);

            return checkPassword;
        }

        [HttpPost]
        public async Task<bool> AddUser(Guid tenantId, string firstName, string lastName, string email)
        {
            var result = false;
            string firstPassword = StringExtensionsHelper.GenerateRandomPassword();
            var firstChar = StringExtensionsHelper.ConvertTRCharToENChar(firstName).Substring(0, 1).ToLower();
            var lastNameEng = StringExtensionsHelper.ConvertTRCharToENChar(lastName).Replace(" ", "").ToLower();
            var userName = string.Format("{0}_{1}{2}", firstChar, lastNameEng);
            int i = 0;
            var newUserName = userName;

            var users = _context.Users.Where(x => x.UserName == newUserName).ToList();

            if (users.Count > 0)
            {
                newUserName = string.Format("{0}_{1}", userName, users.Count);
            }

            //while (userService.UserManager.Users.Any(p => p.UserName == newUserName))
            //{
            //    i++;
            //    newUserName = string.Format("{0}_{1}", userName, i);
            //}

            var usr = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email.Trim(),
                UserName = newUserName,
                Name = firstName,
                LastName = lastName,
                ChangePassword = false,
                EmailConfirmed = true,
                IsActive = true
            };

            var userResult = UserManager.CreateAsync(usr, firstPassword);

            return result;
        }


        //Role crud
        //User crud
        //Permissions
    }
}
