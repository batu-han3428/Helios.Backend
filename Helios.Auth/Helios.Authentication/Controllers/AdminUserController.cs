using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Helpers;
using Helios.Authentication.Models;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System;
using System.Net.Sockets;
using System.Text.Encodings.Web;
using Helios.Authentication.Services.Interfaces;

namespace Helios.Authentication.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminUserController : Controller
    {
        private AuthenticationContext _context;
        readonly UserManager<ApplicationUser> _userManager;
        private readonly IBus _backgorundWorker;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBaseService _baseService;
        public AdminUserController(AuthenticationContext context, UserManager<ApplicationUser> userManager, IBus _bus, RoleManager<ApplicationRole> roleManager, IHttpContextAccessor contextAccessor, IBaseService baseService)
        {
            _context = context;
            _userManager = userManager;
            _backgorundWorker = _bus;
            _roleManager = roleManager;
            _contextAccessor = contextAccessor;
            _baseService = baseService;
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
                Email = user.Email,
                FirstName = user.Name, 
                LastName = user.Name
            };

            return result;
        }

        [HttpPost]
        public async Task<dynamic> AddUser(UserDTO model)
        {
            //burası güncellenecek. şimdilik userın girdiği şifre kaydediliyor.
            try
            {
                if (await _userManager.FindByEmailAsync(model.Email) == null)
                {
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

                    var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == model.Role.ToString());

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
                        PhotoBase64String = "",
                        LastChangePasswordDate = DateTime.Now
                    };

                    usr.UserRoles.Add(new ApplicationUserRole
                    {
                        Role = role,
                        User = usr,
                        RoleId = role.Id,
                        UserId = usr.Id,
                    });

                    if (model.Role == Enums.Roles.StudyUser)
                    {
                        //studyuser ve rolüne de ekleme yapılmalı
                    }

                    var password = StringExtensionsHelper.GenerateRandomPassword();

                    var userResult = await _userManager.CreateAsync(usr, password);

                    if (userResult.Succeeded)
                    {

                        string tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/ConfirmationMail_TR.html");

                        //string researchName = research.Name;
                        //if (research.IsDemo)
                        //{
                        //    researchName = research.Name.Replace("DEMO-", "");
                        //}
                        string mailContent = "";

                        var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailPhotos/helios_222_70.png");
                        byte[] imageArray = System.IO.File.ReadAllBytes(imgPath);
                        string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                        //var mailSubject = HeliosResource.js_MailConfirmationSbjMsg.Replace("{researchname}", researchName);
                        //if (!string.IsNullOrEmpty(mailSbj))
                        //{
                        //    mailSubject = mailSbj;
                        //}


                        using (StreamReader reader = System.IO.File.OpenText(tempPath))
                        {
                            mailContent = reader.ReadToEnd()
                                .Replace("@FullName", usr.Name + " " + usr.LastName)
                                .Replace("@UserName", usr.Email)
                                .Replace("@Password", password)
                                .Replace("@ResearchName", "Şimdilik çalışma yok")
                                .Replace("@ContactLink", string.Format("{0}://{1}", _contextAccessor.HttpContext.Request.Scheme, _contextAccessor.HttpContext.Request.Host.Value) + "/Account/ContactUs")
                                .Replace("@StudyWebLink", string.Format("{0}://{1}", _contextAccessor.HttpContext.Request.Scheme, _contextAccessor.HttpContext.Request.Host.Value) + "/Account/Login")
                                .Replace("@imgbase64", base64ImageRepresentation)
                                    .Replace("@dynamicdomain", "https://localhost:44458/");
                        }

                        await _baseService.SendMail(usr.Email, "Aktivasyon", mailContent);

                        return new { isSuccess = true, message = "Kullanıcı oluşturuldu. Mail gönderildi" };
                    }

                    return new { isSuccess = false, message = "İşlem başarısız" };
                }
                else
                {
                    return new { isSuccess = false, message = "Kullanıcı zaten kayıtlı" };
                }
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
