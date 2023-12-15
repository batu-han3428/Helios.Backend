using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Enums;
using Helios.Authentication.Helpers;
using Helios.Authentication.Models;
using Helios.Authentication.Services.Interfaces;
using Helios.Common.DTO;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Helios.Common.Model;
using System.Web;

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
        private readonly IEmailService _emailService;
        private readonly ITokenHandler _tokenHandler;
        public AuthAccountController(AuthenticationContext context, UserManager<ApplicationUser> userManager, IBus _bus, IHttpContextAccessor contextAccessor, IBaseService baseService, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, IEmailService emailService, ITokenHandler tokenHandler)
        {
            _context = context;
            _userManager = userManager;
            _backgorundWorker = _bus;
            _contextAccessor = contextAccessor;
            _baseService = baseService;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
            _tokenHandler = tokenHandler;
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

                    if (!checkPassword)
                    {
                        user.AccessFailedCount++;
                        var remainAttemp = 5 - user.AccessFailedCount;
                        var failMsg = user.AccessFailedCount == 4 ? "If you enter your password incorrectly, your account will be blocked, please contact the system administrator." : "You have @Change attempts left for the validity of your password.";
                        var updateResult = await _userManager.UpdateAsync(user);

                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = false,
                            Message = failMsg,
                            Values = user.AccessFailedCount == 4 ? null : new { Change = remainAttemp.ToString() }
                        };
                    }

                    var passUpdateDate = user.LastChangePasswordDate.AddMonths(6);
                    var now = DateTime.UtcNow;
                    var returnMessage = "";

                    if (now >= passUpdateDate)
                    {

                        user.ChangePassword = false;
                        returnMessage = "Dear @Change. Your password has expired. Password reset e-mail has been sent to your e-mail address, please follow the steps to renew your password.";
                    }

                    if (!user.ChangePassword)
                    {
                        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var username = user.Email;

                        if (string.IsNullOrEmpty(returnMessage))
                        {
                            return new ApiResponse<dynamic>
                            {
                                IsSuccess = false,
                                Message = returnMessage,
                                Values = new { redirect = $"/reset-password/code={HttpUtility.UrlEncode(code)}/username={username}" }
                            };
                        }
                        else
                        {
                            if (!user.IsResetPasswordMailSent)
                            {
                                await _emailService.ForgotPasswordMail(new ForgotPasswordDTO
                                {
                                    Name = user.Name,
                                    LastName = user.LastName,
                                    Mail = user.Email,
                                    Code = code,
                                    Language = model.Language
                                });
                            }

                            return new ApiResponse<dynamic>
                            {
                                IsSuccess = false,
                                Message = returnMessage,
                                Values = new { Change = user.Email }
                            };
                        }
                    }

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        Token token = _tokenHandler.CreateAccessToken(user);
                        user.RefreshToken = token.RefreshToken;
                        user.RefrestTokenEndDate = token.Expiration.AddMinutes(3);
                        int tokenResult = await _context.SaveChangesAsync();

                        if (tokenResult > 0)
                        {
                            return new ApiResponse<dynamic>
                            {
                                IsSuccess = true,
                                Message = "",
                                Values = new { accessToken = token.AccessToken }
                            };
                        }
                        else
                        {
                            return new ApiResponse<dynamic>
                            {
                                IsSuccess = true,
                                Message = "Unsuccessful"
                            };
                        }
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
        public async Task<dynamic> SaveForgotPassword(string Mail, string Language)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == Mail);

                if (user == null)
                {
                    return new ApiResponse<dynamic> { IsSuccess = false, Message = "User not registered!" };
                }

                if (!user.IsActive)
                {
                    return new ApiResponse<dynamic> { IsSuccess = false, Message = "User is inactive!" };
                }

                if (user.AccessFailedCount > 4)
                {
                    return new ApiResponse<dynamic> { IsSuccess = false, Message = "Your account has been deactivated because you have logged in incorrectly 5 times. Please contact the system administrator." };
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                await _emailService.ForgotPasswordMail(new ForgotPasswordDTO
                {
                    Name = user.Name,
                    LastName = user.LastName,
                    Mail = user.Email,
                    Code = code,
                    Language = Language
                });

                return new ApiResponse<dynamic> { IsSuccess = true, Message = "Successful" };
            }
            catch (Exception e)
            {
                return new ApiResponse<dynamic> { IsSuccess = false, Message = "An unexpected error occurred." };
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
        public async Task<ApiResponse<dynamic>> ResetPasswordGet(string code, string username, bool firstPassword)
        {
            var user = await _userManager.FindByEmailAsync(username);
            if (user == null)
            {
                return new ApiResponse<dynamic> { IsSuccess = false, Message = "User not found!" };
            }
            else
            {
                var tokenProvider = _userManager.Options.Tokens.PasswordResetTokenProvider;
                code = HttpUtility.UrlDecode(code);
                code = code.Replace(' ', '+');
                var tokenIsValid = await _userManager.VerifyUserTokenAsync(user, tokenProvider, "ResetPassword", code);
                if (!tokenIsValid)
                {
                    var msg = $"The operation has timed out because you have used this link before.";
                    return new ApiResponse<dynamic> { IsSuccess = false, Message = msg };
                }
            }

            return new ApiResponse<dynamic> { IsSuccess = true, Message = "Successful", Values = new { Code = code, Username = username, IsFirstPassword = firstPassword } };
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> ResetPasswordPost(ResetPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new ApiResponse<dynamic> { IsSuccess = false, Message = "User not found!" };
            }
            var passwordValidator = new PasswordValidator<ApplicationUser>();
            var validPassword = await passwordValidator.ValidateAsync(_userManager, user, model.Password);
            if (!validPassword.Succeeded)
            {
                return new ApiResponse<dynamic> { IsSuccess = false, Message = "Invalid password!" };
            }

            var chknewPassHashWthOldPass = await _userManager.CheckPasswordAsync(user, model.Password);

            if (chknewPassHashWthOldPass)
            {
                return new ApiResponse<dynamic> { IsSuccess = false, Message = "The new password cannot be the same as the old password." };
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (result.Succeeded)
            {
                user.ChangePassword = true;
                user.LastChangePasswordDate = DateTime.UtcNow;
                user.EmailConfirmed = true;
                user.IsResetPasswordMailSent = false;
                var passwordResult = await _userManager.UpdateAsync(user);
                if (passwordResult.Succeeded)
                {
                    return new ApiResponse<dynamic> { IsSuccess = true, Message = "Successful" };
                };
            }

            return new ApiResponse<dynamic> { IsSuccess = false, Message = "An unexpected error occurred." };
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

            if (!string.IsNullOrEmpty(profileViewModel.Password))
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

        #region SSO
        [HttpGet]
        public async Task<List<TenantUserModel>> GetUserTenantList(Guid userId)
        {
            if (userId != Guid.Empty)
            {
                return await _context.TenantAdmins.Where(x => x.IsActive && !x.IsDeleted && x.AuthUserId == userId).Include(x => x.Tenant).Select(x => new TenantUserModel
                {
                    TenantId = x.Tenant.Id,
                    Name = x.Tenant.Name
                }).ToListAsync();
            }
            else
            {
                return new List<TenantUserModel>();
            }
        }
        #endregion
    }
}
