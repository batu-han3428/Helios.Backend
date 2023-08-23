using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Helpers;
using Helios.Authentication.Models;
using Helios.Authentication.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace Helios.Authentication.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthAccountController : Controller
    {
        private AuthenticationContext _context;
        readonly UserManager<ApplicationUser> _userManager;
        private readonly IBus _backgorundWorker;       
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBaseService _baseService;
        public AuthAccountController(AuthenticationContext context, UserManager<ApplicationUser> userManager, IBus _bus, IHttpContextAccessor contextAccessor, IBaseService baseService)
        {
            _context = context;
            _userManager = userManager;
            _backgorundWorker = _bus;
            _contextAccessor = contextAccessor;
            _baseService = baseService;
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


        //Role crud

        #region User
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

        [HttpPost]
        public async Task ContactUsMailAsync(ContactUsDTO contactUsDTO)
        {
            string tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/ContactUsMailLayout.html");
            string mailContent = "";

            var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailPhotos/helios_222_70.png");
            byte[] imageArray = System.IO.File.ReadAllBytes(imgPath);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            var mailSubject = contactUsDTO.Subject;

            using (StreamReader reader = System.IO.File.OpenText(tempPath))
            {
                mailContent = reader.ReadToEnd()
                    .Replace("@userName", contactUsDTO.Name)
                    .Replace("@email", contactUsDTO.Email)
                    .Replace("@institution", contactUsDTO.Company)
                    .Replace("@studycode", contactUsDTO.StudyCode)
                    .Replace("@usermessage", contactUsDTO.Message)
                    .Replace("@imgbase64", base64ImageRepresentation)
                    .Replace("@dynamicdomain", string.Format("{0}://{1}", _contextAccessor.HttpContext.Request.Scheme, _contextAccessor.HttpContext.Request.Host.Value));
            }
           
            for (int i = 0; i < contactUsDTO.MailAddresses.Count; i++)
            {
                await _baseService.SendMail(contactUsDTO.MailAddresses[i].ToString(), mailSubject, mailContent);
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

        #endregion
        //Permissions
    }
}
