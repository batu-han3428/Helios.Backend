using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Helpers;
using Helios.Authentication.Models;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

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
        private readonly SmtpClient _smtpClient;
        public AuthAccountController(AuthenticationContext context, UserManager<ApplicationUser> userManager, IBus _bus/*, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnv*/, IHttpContextAccessor contextAccessor, SmtpClient smtpClient)
        {
            _context = context;
            _userManager = userManager;
            _backgorundWorker = _bus;
            _contextAccessor = contextAccessor;
            _smtpClient = smtpClient;
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
                var mailMessage = new MailMessage("accounts@helios-crf.com", contactUsDTO.MailAddresses[i].ToString(), mailSubject, mailContent)
                { IsBodyHtml = true, Sender = new MailAddress("accounts@helios-crf.com") };

                var isSend = _smtpClient.SendMailAsync(mailMessage);
                isSend.Wait();
            }
        }

        #endregion
        //Permissions
    }
}
