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
using Google.Protobuf.WellKnownTypes;
using Helios.Common.DTO;
using Org.BouncyCastle.Asn1.Crmf;
using Helios.Authentication.Enums;
using Newtonsoft.Json.Linq;
using Helios.Common.Model;
using MassTransit.Initializers;

namespace Helios.Authentication.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminUserController : Controller
    {
        private AuthenticationContext _context;
        readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBaseService _baseService;
        private readonly IEmailService _emailService;
        public AdminUserController(AuthenticationContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IHttpContextAccessor contextAccessor, IBaseService baseService, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _contextAccessor = contextAccessor;
            _baseService = baseService;
            _emailService = emailService;
        }

        #region Tenant
        [HttpPost]
        public async Task<bool> AddTenant(TenantModel model)
        {
            _context.Tenants.Add(new Tenant
            {
                AddedById = model.UserId,
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
            var tenant = await _context.Tenants.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (tenant == null)
            {
                return false;
            }

            tenant.Name = model.Name;
            tenant.UpdatedAt = DateTimeOffset.Now;
            tenant.UpdatedById = model.UserId;

            _context.Tenants.Update(tenant);
            //var result = await _context.SaveAuthenticationContextAsync(new Guid(), DateTimeOffset.Now) > 0;
            //var result = _context.SaveChanges() > 0;

            return true;
        }

        [HttpGet]
        public async Task<TenantModel> GetTenant(Guid id)
        {
            var model = new TenantModel();

            var tenant = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == id && x.IsActive && !x.IsDeleted);

            if (tenant != null)
            {
                model.Id = tenant.Id;
                model.Name = tenant.Name;
            }

            return model;
        }

        [HttpGet]
        public async Task<List<TenantModel>> GetTenantList()
        {
            try
            {
                //await _backgorundWorker.Publish(new UserDTO()
                //{
                //    TenantId = Guid.NewGuid()
                //});

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
        public async Task<ApiResponse<dynamic>> UpdateUser(AspNetUserDTO model)
        {
            try
            {
                if (_userManager.Users.Any(x => x.Id != model.Id && x.IsActive && x.Email == model.Email))
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "The e-mail address you entered is registered. Please try again."
                    };
                }

                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (user != null)
                {
                    string? oldEmail = user.Email;
                    user.Name = model.Name;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                   
                    await _userManager.UpdateAsync(user);

                    if (oldEmail != model.Email)
                    {
                        await _emailService.UserResetPasswordMail(new StudyUserModel
                        {
                            Name = user.Name,
                            LastName = user.LastName,
                            Email = user.Email
                        });
                    }

                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "An unexpected error occurred."
                    };
                }
            }
            catch (Exception)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };
            }  
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

        [HttpGet]
        public async Task<List<AspNetUserDTO>> GetUserList(string AuthUserIds)
        {
            string[] authUserIdsArray = AuthUserIds.Split(',');
            List<Guid> authUserIds = new List<Guid>();
            foreach (string id in authUserIdsArray)
            {
                if (Guid.TryParse(id, out Guid guid))
                {
                    authUserIds.Add(guid);
                }
            }
            return await _userManager.Users.Where(x => x.IsActive && authUserIds.Contains(x.Id)).Select(x => new AspNetUserDTO
            {
                Id = x.Id,
                Email = x.Email,
                Name = x.Name,
                LastName = x.LastName,
                IsActive = x.IsActive,
            }).ToListAsync();
        }

        [HttpGet]
        public async Task<AspNetUserDTO> GetUser(string email)
        {
            return await _userManager.Users.Where(x => x.IsActive && x.Email == email).Select(x => new AspNetUserDTO
            {
                Id = x.Id,
                Email = x.Email,
                Name = x.Name,
                LastName = x.LastName,
                IsActive = x.IsActive,
            }).FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<ApiResponse<StudyUserDTO>> AddStudyUser(StudyUserModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) == null)
            {
                var firstChar = StringExtensionsHelper.ConvertTRCharToENChar(model.Name).Substring(0, 1).ToLower();
                var lastNameEng = StringExtensionsHelper.ConvertTRCharToENChar(model.LastName).Replace(" ", "").ToLower();
                var userName = string.Format("{0}_{1}{2}", model.TenantId, firstChar, lastNameEng);

                var newUserName = userName;
                int i = 0;

                while (await _userManager.Users.AnyAsync(x => x.UserName == newUserName))
                {
                    i++;
                    newUserName = string.Format("{0}_{1}", userName, i);
                }

                var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == Roles.StudyUser.ToString());

                var usr = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    Email = model.Email.Trim(),
                    UserName = newUserName,
                    Name = model.Name,
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
                    StudyId = model.StudyId,
                    TenantId = model.TenantId,
                });

                var password = StringExtensionsHelper.GenerateRandomPassword();

                var userResult = await _userManager.CreateAsync(usr, password);

                if (userResult.Succeeded)
                {
                    return new ApiResponse<StudyUserDTO>
                    {
                        IsSuccess = true,
                        Message = "",
                        Values = new StudyUserDTO { AuthUserId = usr.Id, Password = password }
                    };
                }

                return new ApiResponse<StudyUserDTO>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }
            else
            {
                return new ApiResponse<StudyUserDTO>
                {
                    IsSuccess = false,
                    Message = "This user is already registered in the system.",
                };
            }
        }

        [HttpPost]
        public async Task AddStudyUserMail(StudyUserModel model)
        {
            await _emailService.AddStudyUserMail(model);
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> UserResetPassword(StudyUserModel model)
        {
            if (model.AuthUserId != Guid.Empty)
            {
                var user = await _userManager.FindByIdAsync(model.AuthUserId.ToString());

                if (user == null)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "An unexpected error occurred."
                    };
                }else if (!user.IsActive)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Please activate the account first and then try this process again."
                    };
                }else
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
                                model.Password = newPassword;
                                await _emailService.UserResetPasswordMail(model);
                                return new ApiResponse<dynamic>
                                {
                                    IsSuccess = true,
                                    Message = "Successful"
                                };
                            }
                        }
                    }
                }
            }
            return new ApiResponse<dynamic>
            {
                IsSuccess = false,
                Message = "An unexpected error occurred."
            };
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetSystemAdminUser(SystemAdminDTO model)
        {
            if (model.Id == Guid.Empty)
            {
                if (await _userManager.FindByEmailAsync(model.Email) == null)
                {
                    var firstChar = StringExtensionsHelper.ConvertTRCharToENChar(model.Email).Substring(0, 1).ToLower();
                    var lastNameEng = StringExtensionsHelper.ConvertTRCharToENChar(model.Email).Replace(" ", "").ToLower();
                    var userName = string.Format("{0}_{1}{2}", Guid.NewGuid(), firstChar, lastNameEng);

                    var newUserName = userName;
                    int i = 0;

                    while (await _userManager.Users.AnyAsync(x => x.UserName == newUserName))
                    {
                        i++;
                        newUserName = string.Format("{0}_{1}", userName, i);
                    }

                    var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == Roles.SystemAdmin.ToString());

                    var usr = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        Email = model.Email.Trim(),
                        UserName = newUserName,
                        Name = model.Email.Trim(),
                        LastName = model.Email.Trim(),
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
                        StudyId = Guid.Empty,
                        TenantId = Guid.Empty,
                    });

                    var password = StringExtensionsHelper.GenerateRandomPassword();

                    var userResult = await _userManager.CreateAsync(usr, password);

                    if (userResult.Succeeded)
                    {
                        _context.SystemAdmins.Add(new SystemAdmin
                        {
                            AuthUserId = usr.Id,
                        });

                        var result = await _context.SaveAuthenticationContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                        if (result)
                        {
                            model.Password = password;
                            await _emailService.SystemAdminUserMail(model);
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
                        Message = "Unsuccessful"
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "This user is already registered in the system.",
                    };
                }
            }

            return new ApiResponse<dynamic>
            {
                IsSuccess = false,
                Message = "This user is already registered in the system.",
            };
        }

        [HttpGet]
        public async Task<List<SystemUserModel>> GetSystemAdminUserList()
        {
            return await (
                from systemAdmin in _context.SystemAdmins
                join userManagerUser in _userManager.Users on systemAdmin.AuthUserId equals userManagerUser.Id
                where !systemAdmin.IsDeleted
                select new SystemUserModel
                {
                    Id = userManagerUser.Id,
                    Email = userManagerUser.Email,
                    IsActive = systemAdmin.IsActive
                }
            ).ToListAsync();
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SystemAdminActivePassive(SystemAdminDTO model)
        {
            var user = await _context.SystemAdmins.FirstOrDefaultAsync(x => x.AuthUserId == model.Id);

            if (user != null)
            {
                user.IsActive = !user.IsActive;

                var result = await _context.SaveAuthenticationContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                if (result)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }              
            }
            else
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };
            }
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SystemAdminResetPassword(SystemAdminDTO model)
        {
            if (model.Id != Guid.Empty)
            {
                var user = await _userManager.FindByIdAsync(model.Id.ToString());

                if (user == null)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "An unexpected error occurred."
                    };
                }
                else if (!user.IsActive)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Please activate the account first and then try this process again."
                    };
                }
                else
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
                                model.Password = newPassword;
                                model.isAddUser = false;
                                await _emailService.SystemAdminUserMail(model);
                                return new ApiResponse<dynamic>
                                {
                                    IsSuccess = true,
                                    Message = "Successful"
                                };
                            }
                        }
                    }
                }
            }
            return new ApiResponse<dynamic>
            {
                IsSuccess = false,
                Message = "An unexpected error occurred."
            };
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SystemAdminDelete(SystemAdminDTO model)
        {
            var user = await _context.SystemAdmins.FirstOrDefaultAsync(x => x.AuthUserId == model.Id);

            if (user != null)
            {
                _context.SystemAdmins.Remove(user);

                var result = await _context.SaveAuthenticationContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                if (result)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            else
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };
            }
        }
        #endregion
    }
}
