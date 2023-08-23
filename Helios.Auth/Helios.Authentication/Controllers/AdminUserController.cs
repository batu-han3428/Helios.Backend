using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Helpers;
using Helios.Authentication.Models;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helios.Authentication.Controllers
{
    public class AdminUserController : Controller
    {
        private AuthenticationContext _context;
        readonly UserManager<ApplicationUser> _userManager;
        private readonly IBus _backgorundWorker;

        public AdminUserController(AuthenticationContext context, UserManager<ApplicationUser> userManager, IBus _bus)
        {
            _context = context;
            _userManager = userManager;
            _backgorundWorker = _bus;
        }

        #region Tenant
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

        [HttpPost]
        public async Task<bool> UpdateTenant(TenantModel model)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == model.Id && x.IsActive && !x.IsDeleted);

            if(tenant == null)
            {
                return false;
            }

            tenant.Name = model.Name;
            _context.Tenants.Update(tenant);
            var result = await _context.SaveAuthenticationContextAsync(new Guid(), DateTimeOffset.Now) > 0;

            return result;
        }

        [HttpGet]
        public async Task<List<TenantModel>> GetTenantList()
        {
            try
            {
                await _backgorundWorker.Publish(new UserDTO()
                {
                    TenantId = Guid.NewGuid()
                });

                var result = await _context.Tenants.Where(x => x.IsActive && !x.IsDeleted).Select(x => new TenantModel()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        #endregion

        #region User crud

        [HttpGet]
        public async Task<UserDTO> GetUserByEmail(string mail)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Email == mail);
            
            if (user == null)
                return new UserDTO();

            var result = new UserDTO()
            {
                UserId = user.Id,
                TenantId = user.TenantId,
                Email = user.Email,
                FirstName = user.Name, 
                LastName = user.Name
            };

            return result;
        }

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
    }
}
