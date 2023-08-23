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

    }
}
