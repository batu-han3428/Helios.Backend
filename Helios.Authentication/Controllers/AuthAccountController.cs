using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Helpers;
using Helios.Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Principal;

namespace Helios.Authentication.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthAccountController : Controller
    {
        private AuthenticationContext _context;
        readonly UserManager<ApplicationUser> _userManager;

        public AuthAccountController(AuthenticationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        //public async Task<bool> AddRole(Guid tenantId, string firstName, string lastName, string email)
        //{
        //    var result = false;

        //    return result;
        //}

        //[HttpPost]
        //public async Task<bool> Login(AccountModel model)
        //{
        //    var user = await UserManager.Users.Where(p => p.Email == model.Email).FirstOrDefaultAsync();

        //    if (user == null) { return false; }

        //    var checkPassword = await UserManager.CheckPasswordAsync(user, model.Password);

        //    return checkPassword;
        //}

        //[HttpPost]
        //public async Task<bool> AddUser(Guid tenantId, string firstName, string lastName, string email)
        //{
        //    var result = false;
        //    string firstPassword = StringExtensionsHelper.GenerateRandomPassword();
        //    var firstChar = StringExtensionsHelper.ConvertTRCharToENChar(firstName).Substring(0, 1).ToLower();
        //    var lastNameEng = StringExtensionsHelper.ConvertTRCharToENChar(lastName).Replace(" ", "").ToLower();
        //    var userName = string.Format("{0}_{1}{2}", firstChar, lastNameEng);
        //    int i = 0;
        //    var newUserName = userName;

        //    var users = _context.Users.Where(x => x.UserName == newUserName).ToList();

        //    if (users.Count > 0)
        //    {
        //        newUserName = string.Format("{0}_{1}", userName, users.Count);
        //    }

        //    //while (userService.UserManager.Users.Any(p => p.UserName == newUserName))
        //    //{
        //    //    i++;
        //    //    newUserName = string.Format("{0}_{1}", userName, i);
        //    //}

        //    var usr = new ApplicationUser
        //    {
        //        Id = Guid.NewGuid(),
        //        Email = email.Trim(),
        //        UserName = newUserName,
        //        Name = firstName,
        //        LastName = lastName,
        //        ChangePassword = false,
        //        EmailConfirmed = true,
        //        IsActive = true,
        //    };

        //    var userResult = UserManager.CreateAsync(usr, firstPassword);

        //    return result;
        //}

        //Tenant crud
        [HttpPost]
        public async Task<bool> AddTenant(TenantModel model)
        {
            _context.Tenants.Add(new Tenant
            {
                Name = model.Name,
                CreatedAt = DateTimeOffset.Now
            });

            //var result = _context.SaveAuthenticationContext(new Guid(), DateTimeOffset.Now) > 0;
            var result = _context.SaveChanges() > 0;

            return result;
        }


        //Role crud

        #region User crud
        [HttpPost]
        public async Task<bool> AddUser(UserDTO model)
        {
            //burası güncellenecek. şimdilik userın girdiği şifre kaydediliyor.
            try
            {
                var result = false;
                //string firstPassword = StringExtensionsHelper.GenerateRandomPassword();
                var firstChar = StringExtensionsHelper.ConvertTRCharToENChar(model.FirstName).Substring(0, 1).ToLower();
                var lastNameEng = StringExtensionsHelper.ConvertTRCharToENChar(model.LastName).Replace(" ", "").ToLower();
                var userName = string.Format("{0}_{1}{2}", model.TenantId, firstChar, lastNameEng);

                var newUserName = userName;
                int i = 0;

                while (await _userManager.Users.AnyAsync(x => x.UserName == newUserName))
                {
                    i++;
                    newUserName = string.Format("{0}_{1}", userName, i);
                }

                var usr = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    Email = model.Email.Trim(),
                    UserName = newUserName,
                    Name = model.FirstName,
                    LastName = model.LastName,
                    ChangePassword = false,
                    EmailConfirmed = true,
                    IsActive = true,
                    TenantId = model.TenantId,
                    PhotoBase64String = ""
                };

                var userResult = await _userManager.CreateAsync(usr, model.Password);

                if (userResult.Succeeded)
                {
                    result = true;
                }

                return result;
            }
            catch (Exception e)
            {

                throw;
            }
            
        }

        [HttpPost]
        public async Task<bool> PassiveOrActiveUser(UserDTO model)
        {
            var result = false;
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (user != null)
            {
                user.IsActive = !user.IsActive;

                if (user.IsActive)
                    user.AccessFailedCount = 0;

                await _userManager.UpdateAsync(user);
                result = true;
            }
            return result;
        }

        [HttpGet]
        public async Task<bool> SendNewPasswordForUser(Guid userId)
        {
            var result = false;
            if (userId != Guid.Empty)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user != null && user.IsActive)
                {
                    user.ChangePassword = false;
                    var changePassword = await _userManager.UpdateAsync(user);
                    if (changePassword.Succeeded)
                    {
                        string newPassword = StringExtensionsHelper.GenerateRandomPassword();
                        var removeResult = await _userManager.RemovePasswordAsync(user);
                        if (removeResult.Succeeded)
                        {
                            var passResult = await _userManager.AddPasswordAsync(user, newPassword);
                            if (passResult.Succeeded)
                            {
                                //var studyLoginUrl = Url.LoginConfirmationLink(research.ShortName, Request.Scheme);
                                //await emailservice.SendNewPasswordMailAsync(studyLoginUrl, newPassword, user, research, environment, this.httpContextAccessor);
                                //return Json(true);
                                result = true;
                            }
                        }
                    }
                }
            }
            return result;
        }

        [HttpPost]
        public async Task<bool> UpdateUser(UserDTO model)
        {
            var result = false;
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
            if (user != null)
            {
                user.Name = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;

                await _userManager.UpdateAsync(user);
                result = true;
            }
            return result;
        }

        [HttpPost]
        public async Task<bool> UserProfileResetPassword(UserDTO model)
        {
            var result = false;

            if (!String.IsNullOrEmpty(model.Password))
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);

                if (user != null)
                {
                    var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (passwordCheck)
                    {
                        var passwordValidator = new PasswordValidator<ApplicationUser>();
                        var validPassword = await passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                        if (validPassword.Succeeded)
                        {
                            if (model.NewPassword == model.ConfirmPassword)
                            {
                                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                                var resetResult = await _userManager.ResetPasswordAsync(user, code, model.NewPassword);
                                if (resetResult.Succeeded)
                                {
                                    var passwordResult = await _userManager.UpdateAsync(user);
                                    if (passwordResult.Succeeded)
                                    {
                                        result = true;
                                    }
                                }
                            }
                        } 
                    }
                }
            }

            return result;
        }
        

        #endregion
        //Permissions
    }
}
