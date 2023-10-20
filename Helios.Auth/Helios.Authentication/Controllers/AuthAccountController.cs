using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Enums;
using Helios.Authentication.Helpers;
using Helios.Authentication.Models;
using Helios.Authentication.Services.Interfaces;
using Helios.Common.DTO;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math;
using System;
using System.Configuration;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Xml.Linq;
using System.Net.Mime;

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
        private readonly IConfiguration _configuration;
        public AuthAccountController(AuthenticationContext context, UserManager<ApplicationUser> userManager, IBus _bus, IHttpContextAccessor contextAccessor, IBaseService baseService, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _backgorundWorker = _bus;
            _contextAccessor = contextAccessor;
            _baseService = baseService;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
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
                    return new { isSuccess = true, message = "Successful" };
                }
                return new { isSuccess = false, message = "Unsuccessful" };
            }

            return new { isSuccess = false, message = "This role already exists." };
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
                            return new { isSuccess = true, message = "Successful" };
                        }
                        return new { isSuccess = false, message = "Unsuccessful" };
                    }
                    return new { isSuccess = false, message = "The role is already defined in the user." };
                }
                return new { isSuccess = false, message = "User not found." };
            }
            catch (Exception e)
            {

                throw;
            }
          
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> Login(AccountModel model)
        {
            try
            {
                var user = _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).FirstOrDefault(p => p.Email == model.Email);
                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = false,
                            Message = "Please contact the system administrator to open your account."
                        };
                    }

                    if (user.AccessFailedCount == 5)
                    {
                        user.IsActive = false;
                        var updateResult = await _userManager.UpdateAsync(user);
                        if (updateResult.Succeeded)
                        {
                            return new ApiResponse<dynamic>
                            {
                                IsSuccess = false,
                                Message = "Your account has been locked because you exceeded the login attempt limit. Please contact the system administrator to open your account."
                            };
                        }
                    }

                    var checkPassword = await _userManager.CheckPasswordAsync(user, model.Password);

                    //if (!checkPassword)
                    //{
                    //    user.AccessFailedCount++;
                    //    var remainAttemp = 5 - user.AccessFailedCount;
                    //    var failMsg = user.AccessFailedCount == 4 ? "Eğer şifrenizi yanlış girerseniz hesabınız bloke olacaktır, lütfen sistem yöneticisi ile iletişime geçiniz." : "Şifrenizin kullanım geçerliliği için @Number deneme hakkınız kalmıştır.".Replace("@Number", remainAttemp.ToString());
                    //    var updateResult = await _userManager.UpdateAsync(user);

                    //    return new ApiResponse<dynamic>
                    //    {
                    //        IsSuccess = false,
                    //        Message = failMsg
                    //    };
                    //}

                    //var passUpdateDate = user.LastChangePasswordDate.AddMonths(6);
                    //var now = DateTime.UtcNow;
                    //var returnMessage = "";
                    //bool firstLogin = true;
                    ////if (user.LastChangePasswordDate >= passUpdateDate)
                    //if (now >= passUpdateDate)
                    //{
                    //    firstLogin = false;
                    //    user.ChangePassword = false;
                    //    returnMessage = "Sayın @UserFullName,\r\n\r\nŞifrenizin kullanım süresi dolmuştur.\r\nYeni şifrenizi oluşturmak için aşağıda bulunan \"Yeni şifre gönder\" butonuna tıklayınız.".Replace("@UserFullName", user.Email);
                    //}

                    //if (!user.ChangePassword)
                    //{
                    //    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    //    var username = user.Email;
                    //    //return RedirectToAction("ResetPassword", "Account", new { firstlogin = firstLogin, code = code, username = username, firstPassword = true, shortName, returnMessage = returnMessage });
                    //    return new ApiResponse<dynamic>
                    //    {
                    //        IsSuccess = false,
                    //        Message = "ResetPassword git",
                    //        Values = new { code = code, username = username, firstPassword = true }
                    //    };
                    //}

                    //var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);

                    if (/*result.Succeeded*/true)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        var enumList = userRoles.Select(x => Enum.Parse<Roles>(x))
                        .ToList();

                        TokenHandler tokenHandler = new TokenHandler(_configuration);
                        Token token = tokenHandler.CreateAccessToken(user);
                        user.RefreshToken = token.RefreshToken;
                        user.RefrestTokenEndDate = token.Expiration.AddMinutes(3);
                        await _context.SaveChangesAsync();

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
                                //return new { isSuccess = true, message = "1. sayfa" };
                            case var _ when
                            enumList.Contains(Roles.TenantAdmin):
                                return new ApiResponse<dynamic>
                                {
                                    IsSuccess = true,
                                    Message = "2. sayfa",
                                    Values = new { accessToken = token.AccessToken }
                                };
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
                                //return new { isSuccess = true, message = "3. sayfa" };
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

                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = true,
                            Message = "Successful"
                        };
                    }
                }

                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Invalid user"
                };
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

        [HttpPost]
        public async Task<ApiResponse<dynamic>> ContactUsMail(ContactUsModel contactUsModel)
        {
            try
            {
                string tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/ContactUsMailLayout.html");
                string mailContent = "";

                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailPhotos/helios_222_70.jpg");
                byte[] imageArray = System.IO.File.ReadAllBytes(imgPath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                var mailSubject = contactUsModel.Subject;

                using (StreamReader reader = System.IO.File.OpenText(tempPath))
                {
                    mailContent = reader.ReadToEnd()
                        .Replace("@userName", contactUsModel.NameSurname)
                        .Replace("@email", contactUsModel.Email)
                        .Replace("@institution", contactUsModel.InstitutionName)
                        .Replace("@studycode", contactUsModel.StudyCode)
                        .Replace("@usermessage", contactUsModel.YourMessage)
                        .Replace("@imgbase64", base64ImageRepresentation)
                        .Replace("@dynamicdomain", string.Format("{0}://{1}", _contextAccessor.HttpContext.Request.Scheme, _contextAccessor.HttpContext.Request.Host.Value));
                }

                for (int i = 0; i < contactUsModel.MailAddresses.Count; i++)
                {
                    await _baseService.SendMail(contactUsModel.MailAddresses[i].ToString(), mailSubject, mailContent/*, attachment*/);
                }

                return new ApiResponse<dynamic>
                {
                    IsSuccess = true,
                    Message = "Mesajınız gönderildi."
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Beklenmeyen bir hata oluştu."
                };
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


        [HttpGet]
        public async Task<dynamic> UserProfileSetting(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var rolName = await _userManager.GetRolesAsync(user);

                return new
                {
                    isSuccess = true,
                    message = "",
                    values = new
                    {
                        ApplicationUser = user,
                        ProfileViewModel = new ProfileViewModel
                        {
                            Email = user.Email,
                            FirstName = user.Name,
                            LastName = user.LastName,
                            UserName = user.UserName,
                        },
                        ResetPasswordViewModel = new ResetPasswordViewModel
                        {
                            Username = user.UserName,
                            Code = "",
                        },
                        UserRolName = rolName.FirstOrDefault()
                    }
                };
            }
            return new { isSuccess = false, message = "Kullanıcı bulunamadı." };
        }

        [HttpPost]
        public async Task<dynamic> UserNameUpdate(ProfileViewModel profileViewModel)
        {
            if (string.IsNullOrEmpty(profileViewModel.FirstName) || string.IsNullOrEmpty(profileViewModel.LastName))
            {
                return new { isSuccess = false, messsage = "Ad veya Soyad boş bırakılmamalıdır." };
            }
            else
            {
                var user = await _userManager.FindByEmailAsync(profileViewModel.Email);

                if (user != null)
                {
                    user.Name = profileViewModel.FirstName;
                    user.LastName = profileViewModel.LastName;
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        return new { isSuccess = false, messsage = "Güncelleme hatası." };
                    }
                }
                else
                {
                    return new { isSuccess = false, messsage = "Kullanıcı bulunamadı veya hala e-posta onayı bekliyor." };
                }

            }

            if (!String.IsNullOrEmpty(profileViewModel.Password))
            {
                var user = await _userManager.FindByEmailAsync(profileViewModel.Email);

                if (user == null)
                {
                    return new { isSuccess = false, messsage = "Kullanıcı bulunamadı veya hala e-posta onayı bekliyor." };
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(user, profileViewModel.Password);
                if (!passwordCheck)
                {
                    return new { isSuccess = false, messsage = "Mevcut şifre yanlış." };
                }

                var passwordValidator = new PasswordValidator<ApplicationUser>();
                var validPassword = await passwordValidator.ValidateAsync(_userManager, user, profileViewModel.NewPassword);
                if (!validPassword.Succeeded)
                {
                    string errmes = "";
                    foreach (var item in validPassword.Errors)
                    {
                        errmes += "<br>" + item.Description;
                    }
                    return new { isSuccess = false, messsage = errmes };
                }

                if (profileViewModel.NewPassword != profileViewModel.ConfirmPassword)
                {
                    return new { isSuccess = false, messsage = "Şifre veya onay şifresi eşleşmiyor." };
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, code, profileViewModel.NewPassword);
                if (resetResult.Succeeded)
                {
                    var passwordResult = await _userManager.UpdateAsync(user);
                    if (!passwordResult.Succeeded)
                    {
                        string errmes = "";
                        foreach (var item in passwordResult.Errors)
                        {
                            errmes += "<br>" + item.Description;
                        }
                        return new { isSuccess = false, messsage = errmes };
                    }
                }
            }

            return new { isSuccess = true, messsage = "İşlem başarılı." };
        }
    }
}
