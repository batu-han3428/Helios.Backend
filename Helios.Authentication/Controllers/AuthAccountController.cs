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
    public class AuthAccountController : Controller
    {
        private AuthenticationContext _context;
        UserManager<ApplicationUser> UserManager { get; set; }

        public AuthAccountController(AuthenticationContext context)
        {
            _context = context;
        }
        //public async Task<bool> AddRole(Guid tenantId, string firstName, string lastName, string email)
        //{
        //    var result = false;

        //    return result;
        //}

        [HttpPost]
        public async Task<bool> Login(AccountModel model)
        {
            var user = await UserManager.Users.Where(p => p.Email == model.Email).FirstOrDefaultAsync();

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

        //Tenant crud
        [HttpPost]
        public async Task<bool> AddTenant(TenantModel model)
        {
            _context.Tenants.Add(new Tenant
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                CreatedAt = DateTimeOffset.Now,
                IsActive = true
            });

            var result = await _context.SaveAuthenticationContextAsync(new Guid(), DateTimeOffset.Now) > 0;

            return result;
        }

        [HttpGet]
        public async Task<List<TenantModel>> GetTenantList()
        {
            var result = await _context.Tenants.Where(x => x.IsActive && !x.IsDeleted).Select(x => new TenantModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            return result;
        }


        //Role crud
        //User crud
        //Permissions
    }
}
