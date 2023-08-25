﻿using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Enums;
using Helios.Authentication.Helpers;
using Helios.Authentication.Models;
using Helios.Authentication.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;

namespace Helios.Authentication.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthAccountController : Controller
    {
        private AuthenticationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IBus _backgorundWorker;       
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBaseService _baseService;
        public AuthAccountController(AuthenticationContext context, UserManager<ApplicationUser> userManager, IBus _bus, IHttpContextAccessor contextAccessor, IBaseService baseService, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _backgorundWorker = _bus;
            _contextAccessor = contextAccessor;
            _baseService = baseService;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<dynamic> AddRole(int role)
        {
            var selectRole = (Roles)role;

            bool exists = await _roleManager.RoleExistsAsync(selectRole.ToString());
            if (!exists)
            {
                var result = await _roleManager.CreateAsync(new ApplicationRole { Name = selectRole.ToString() });
                if (result.Succeeded)
                {
                    return new { isSuccess = true, message = "İşlem başarılı" };
                }
                return new { isSuccess = false, message = "İşlem başarısız" };
            }

            return new { isSuccess = false, message = "Rol zaten var" };
        }

        [HttpPost]
        public async Task<dynamic> AddUserRole(string mail, int role, Guid tenantId)
        {
            try
            {
                var user = _userManager.Users.FirstOrDefault(p => p.Email == mail);
                if (user != null)
                {
                    var selectRole = (Roles)role;
                    
                    var existsUserRole = await _userManager.IsInRoleAsync(user, selectRole.ToString());
                    if (!existsUserRole)
                    {
                        var roleDb = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == selectRole.ToString());
                        await _context.UserRoles.AddAsync(new ApplicationUserRole
                        {
                            User= user,
                            UserId = user.Id,
                            Role = roleDb,
                            RoleId = roleDb.Id,
                            TenantId = tenantId
                        });

                        if (selectRole == Roles.StudyUser)
                        {
                            //studyuserroles e de ekleme yapılmalı.
                        }

                        var result = await _context.SaveChangesAsync() > 0;

                        //var result = await _userManager.AddToRoleAsync(user, selectRole.ToString());
                        if (result)
                        {
                            return new { isSuccess = true, message = "Başarılı." };
                        }
                        return new { isSuccess = false, message = "İşlem başarısız." };
                    }
                    return new { isSuccess = false, message = "Kullanıcıda zaten rol tanımlı." };
                }
                return new { isSuccess = false, message = "Kullanıcı bulunamadı." };
            }
            catch (Exception e)
            {

                throw;
            }
          
        }

        [HttpPost]
        public async Task<dynamic> Login(AccountModel model)
        {
            try
            {
                var user = _userManager.Users.FirstOrDefault(p => p.Email == model.Email);
                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        return new { isSuccess = false, message = "Hesabınızın açılması için lütfen sistem yöneticisi ile iletişime geçiniz." };
                    }

                    if (user.AccessFailedCount == 5)
                    {
                        user.IsActive = false;
                        var updateResult = await _userManager.UpdateAsync(user);
                        if (updateResult.Succeeded)
                        {
                            return new { isSuccess = false, message = "Giriş deneme limitinizi aştığınız için hesabınız kitlenmiştir. Yardım için sistem yöneticinize başvurun." };
                        }
                    }

                    var checkPassword = await _userManager.CheckPasswordAsync(user, model.Password);

                    if (!checkPassword)
                    {
                        user.AccessFailedCount++;
                        var remainAttemp = 5 - user.AccessFailedCount;
                        var failMsg = user.AccessFailedCount == 4 ? "Eğer şifrenizi yanlış girerseniz hesabınız bloke olacaktır, lütfen sistem yöneticisi ile iletişime geçiniz." : "Şifrenizin kullanım geçerliliği için @Number deneme hakkınız kalmıştır.".Replace("@Number", remainAttemp.ToString());
                        var updateResult = await _userManager.UpdateAsync(user);
 
                        return new { isSuccess = false, message = failMsg };
                    }

                    var passUpdateDate = user.LastChangePasswordDate.AddMonths(6);
                    var now = DateTime.UtcNow;
                    var returnMessage = "";
                    bool firstLogin = true;
                    //if (user.LastChangePasswordDate >= passUpdateDate)
                    if (now >= passUpdateDate)
                    {
                        firstLogin = false;
                        user.ChangePassword = false;
                        returnMessage = "Sayın @UserFullName,\r\n\r\nŞifrenizin kullanım süresi dolmuştur.\r\nYeni şifrenizi oluşturmak için aşağıda bulunan \"Yeni şifre gönder\" butonuna tıklayınız.".Replace("@UserFullName", user.Email);
                    }

                    if (!user.ChangePassword)
                    {
                        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var username = user.Email;
                        //return RedirectToAction("ResetPassword", "Account", new { firstlogin = firstLogin, code = code, username = username, firstPassword = true, shortName, returnMessage = returnMessage });
                        return new { isSuccess = false, message = "ResetPassword git", values = new { code = code, username = username, firstPassword = true } };
                    }

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        var enumList = userRoles.Select(x => Enum.Parse<Roles>(x))
                         .ToList();

                        switch (enumList)
                        {
                            case var _ when
                            (
                                enumList.Contains(Roles.SuperAdmin)
                                ||
                                enumList.Contains(Roles.SystemAdmin)
                            )
                            ||
                            (
                             (
                                enumList.Contains(Roles.SuperAdmin)
                                ||
                                enumList.Contains(Roles.SystemAdmin)
                             )
                             &&
                               enumList.Contains(Roles.TenantAdmin)
                            ):
                                return new { isSuccess = true, message = "1. sayfa" };
                            case var _ when
                            enumList.Contains(Roles.TenantAdmin):
                                return new { isSuccess = true, message = "2. sayfa" };
                            case var _ when
                            (
                                enumList.Contains(Roles.SuperAdmin)
                                ||
                                enumList.Contains(Roles.SystemAdmin)
                                ||
                                enumList.Contains(Roles.TenantAdmin)
                            )
                            &&
                            (
                                enumList.Contains(Roles.StudyUser)
                                ||
                                enumList.Contains(Roles.StudySubject)
                            ):
                                return new { isSuccess = true, message = "3. sayfa" };
                            case var _ when enumList.Contains(Roles.StudyUser):


                            // altta ki geçersiz. diğer db ye gidip studyusers da ki rollerine bakıp sayısın
                            // örn. _context.studyusers.Where(x=>x.aspnetUserId = user.Id).include(x=>x.studyuserroles).GroupBy(x => x.TenantId).Select(n => new
                            //{
                            //    tenantId = n.Key,
                            //}).ToList();

                            //var tenants = _context.UserRoles.Where(x=>x.UserId == user.Id).GroupBy(x => x.TenantId).Select(n => new
                            //{
                            //    tenantId = n.Key,
                            //}).ToList();
                            //if (tenants.Count > 1)
                            //{
                            //    return new { isSuccess = true, message = "5. sayfa" };
                            //}
                            //else
                            //{
                            //    return new { isSuccess = true, message = "4. sayfa" };
                            //}
                            default:
                                break;
                        }

                        return new { isSuccess = true, message = "Giriş başarılı!" };
                    }
                }

                return new { isSuccess = false, message = "Geçersiz giriş denemesi!" };
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [HttpPost]
        public async Task<dynamic> SaveForgotPassword(string Mail)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == Mail);

                if (user == null)
                {
                    return new { isSuccess = false, message = "Kullanıcı kayıtlı değil" };
                }

                if (!user.IsActive)
                {
                    return new { isSuccess = false, message = "Kullanıcı aktif değil" };
                }

                if (user.AccessFailedCount > 4)
                {
                    return new { isSuccess = false, message = "5 defa hatalı giriş yaptığınız için hesabınız kitlenmiş" };
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);


                //şimdilik localhost yazdım
                string callbackUrl = $"https://localhost:44458/Account/ResetPassword?code={code}&username={user.Email}";

                //string? callbackUrl = Url.Action(
                //    action: "ResetPassword",
                //    controller: "Account",
                //    values: new { user.Id, code, user.Email },
                //    protocol: Request.Scheme,
                //    host: "https://localhost:44458/"
                //);

                string tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/AdminForgotPasswordMail.html");
                string mailContent = "";

                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailPhotos/helios_222_70.png");
                byte[] imageArray = System.IO.File.ReadAllBytes(imgPath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                var mailSubject = "e-CRF reset password";

                using (StreamReader reader = System.IO.File.OpenText(tempPath))
                {
                    mailContent = reader.ReadToEnd()
                    .Replace("@FullName", user.Name + " " + user.LastName)
                    .Replace("@ContactLink", string.Format("{0}://{1}", _contextAccessor.HttpContext.Request.Scheme, _contextAccessor.HttpContext.Request.Host.Value) + "/Account/ContactUs")
                    .Replace("@PasswordLink", HtmlEncoder.Default.Encode(callbackUrl))
                    .Replace("@imgbase64", base64ImageRepresentation)
                    .Replace("@dynamicdomain", string.Format("{0}://{1}", _contextAccessor.HttpContext.Request.Scheme, _contextAccessor.HttpContext.Request.Host.Value));
                }

                await _baseService.SendMail(user.Email, mailSubject, mailContent);

                return new { isSuccess = true, message = "Mail gönderildi" };
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [HttpGet]
        public async Task<dynamic> ResetPasswordGet(string code, string username, bool firstPassword)
        {
            var user = await _userManager.FindByEmailAsync(username);
            if (user == null)
            {
                return new { isSuccess = false, messsage = "Kullanıcı bulunamadı!" };
            }
            else
            {
                var tokenProvider = _userManager.Options.Tokens.PasswordResetTokenProvider;
                code = code.Replace(' ', '+');
                var tokenIsValid = await _userManager.VerifyUserTokenAsync(user, tokenProvider, "ResetPassword", code);
                if (!tokenIsValid)
                {
                    var msg = $"Bu bağlantıyı daha önce kullandığınız için işlem zaman aşımına uğramıştır. <br><br> Aşağıdaki linke tıklayarak yeni şifre talebinde bulunmanız gerekmektedir.";
                    //return RedirectToAction("Login", "Account", new { Area = "Crf.Web.AdminUI", customMessage = msg });
                    return new { isSuccess = false, messsage = msg };
                }
            }
            //var model = new ResetPasswordViewModel { Code = code, Username = username, IsFirstPassword = firstPassword };

            //return View("/Areas/Crf.Web.AdminUI/Views/Account/ResetPassword.cshtml", model);

            return new { isSuccess = true, message = "Başarılı", values = new { Code = code, Username = username, IsFirstPassword = firstPassword } };
        }

        [HttpPost]
        public async Task<dynamic> ResetPasswordPost(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Username);

            if (user == null)
            {
                return new { isSuccess = false, messsage = "Access Denied" };
            }
            var passwordValidator = new PasswordValidator<ApplicationUser>();
            var validPassword = await passwordValidator.ValidateAsync(_userManager, user, model.Password);
            if (!validPassword.Succeeded)
            {
                return new { isSuccess = false, messsage = "Invalid Password" };
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (result.Succeeded)
            {
                user.ChangePassword = true;
                var passwordResult = await _userManager.UpdateAsync(user);
                if (passwordResult.Succeeded)
                {
                    return new { isSuccess = true, messsage = "Invalid Reset Password", values = new { Username = user.UserName, Password = model.Password, RememberMe = false } };
                };
            }

            //foreach (var error in result.Errors)
            //{
            //    if (error.Description == "Invalid token.")
            //    {
            //        var msg = $"{HeliosResource.OperationTimedOut} <br><br> {HeliosResource.RequestPassword}";
            //        return RedirectToAction("Login", "Account", new { Area = "Crf.Web.AdminUI", customMessage = msg });
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("", error.Description);
            //    }

            //}
            return new { isSuccess = false, messsage = "Invalid Reset Password" };
        }

    }
}
