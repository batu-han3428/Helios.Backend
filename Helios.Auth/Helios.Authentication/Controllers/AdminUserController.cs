using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Helpers;
using Helios.Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Helios.Authentication.Services.Interfaces;
using Helios.Common.DTO;
using Helios.Authentication.Enums;
using Helios.Common.Model;
using MassTransit.Initializers;
using Azure.Storage.Blobs;

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
        private readonly IFileStorageHelper _blobStorage;
        public AdminUserController(AuthenticationContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IHttpContextAccessor contextAccessor, IBaseService baseService, IEmailService emailService, IFileStorageHelper blobStorage)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _contextAccessor = contextAccessor;
            _baseService = baseService;
            _emailService = emailService;
            _blobStorage = blobStorage;
        }

        #region Tenant
        [HttpGet]
        public async Task<List<TenantModel>> GetTenantList()
        {
            return await _context.Tenants.Where(x => x.IsActive && !x.IsDeleted).Select(x => new TenantModel()
            {
                Id = x.Id,
                Name = x.Name,
                ActiveStudies = "0",
                StudyLimit = x.StudyLimit,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).ToListAsync();
        }

        [HttpGet]
        public async Task<TenantModel> GetTenant(Int64 tenantId)
        {
            var tenant = await _context.Tenants.Where(x => x.IsActive && !x.IsDeleted && x.Id == tenantId).Select(x => new TenantModel()
            {
                Id = x.Id,
                Name = x.Name,
                TimeZone = x.TimeZone,
                StudyLimit = x.StudyLimit,
                UserLimit = x.UserLimit,
                Path = x.Logo
            }).FirstOrDefaultAsync();

            if (tenant != null)
            {
                if (!string.IsNullOrEmpty(tenant.Path))
                {
                    BlobClient blobClient = await _blobStorage.GetBlob(tenant.Path);
                    var contentType = blobClient.GetProperties();
                    var ddd = contentType.Value.ContentType;
                    if (blobClient != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            await blobClient.DownloadToAsync(ms);
                            tenant.Logo = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
                
                return tenant;
            }

            return new TenantModel();
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetTenant(TenantDTO tenantDTO)
        {
            bool result = false;

            if (tenantDTO.Id == 0)
            {
                if (await _context.Tenants.AnyAsync(x => x.Name == tenantDTO.TenantName && x.IsActive && !x.IsDeleted))
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "This tenant name is already exist.",
                    };
                }

                Tenant tenant = new Tenant()
                {
                    Name = tenantDTO.TenantName,
                    StudyLimit = tenantDTO.StudyLimit,
                    UserLimit = tenantDTO.UserLimit,
                    TimeZone = tenantDTO.TimeZone
                };

                if (tenantDTO.TenantLogo != null && tenantDTO.TenantLogo.Length > 0)
                {
                    string folderPath = "TenantName/";

                    string photoName = Path.GetFileName(tenantDTO.TenantLogo.FileName);

                    string guidSuffix = Guid.NewGuid().ToString();

                    string newPhotoName = $"{Path.GetFileNameWithoutExtension(photoName)}_{guidSuffix}{Path.GetExtension(photoName)}";

                    await _blobStorage.CreateFolderIfNotExist(folderPath);

                    string fullPath = $"{folderPath}{newPhotoName}";

                    bool success = await _blobStorage.Upload(fullPath, tenantDTO.TenantLogo.OpenReadStream());

                    if (success)
                    {
                        tenant.Logo = fullPath;
                    }
                }

                await _context.Tenants.AddAsync(tenant);

                result = await _context.SaveAuthenticationContextAsync(tenantDTO.UserId, DateTimeOffset.Now) > 0;

                tenantDTO.Id = tenant.Id;
            }
            else
            {
                var oldEntity = _context.Tenants.FirstOrDefault(p => p.Id == tenantDTO.Id && p.IsActive && !p.IsDeleted);

                if (oldEntity != null)
                {
                    if (oldEntity.Name != tenantDTO.TenantName)
                    {
                        if (await _context.Tenants.AnyAsync(x => x.Name == tenantDTO.TenantName && x.IsActive && !x.IsDeleted))
                        {
                            return new ApiResponse<dynamic>
                            {
                                IsSuccess = false,
                                Message = "This tenant name is already exist.",
                            };
                        }
                    }

                    if (oldEntity.Name != tenantDTO.TenantName) oldEntity.Name = tenantDTO.TenantName;
                    if(StringExtensionsHelper.NormalizeString(oldEntity.StudyLimit) != StringExtensionsHelper.NormalizeString(tenantDTO.StudyLimit)) oldEntity.StudyLimit = tenantDTO.StudyLimit;
                    if(StringExtensionsHelper.NormalizeString(oldEntity.UserLimit) != StringExtensionsHelper.NormalizeString(tenantDTO.UserLimit)) oldEntity.UserLimit = tenantDTO.UserLimit;
                    if(StringExtensionsHelper.NormalizeString(oldEntity.TimeZone) != StringExtensionsHelper.NormalizeString(tenantDTO.TimeZone)) oldEntity.TimeZone = tenantDTO.TimeZone;
                    if(oldEntity.Logo != (tenantDTO.TenantLogo?.FileName == null ? null : "TenantName/" + tenantDTO.TenantLogo?.FileName))
                    {
                        string? removeFile = oldEntity.Logo;

                        if (tenantDTO.TenantLogo?.Length > 0)
                        {
                            string folderPath = "TenantName/";

                            string photoName = Path.GetFileName(tenantDTO.TenantLogo.FileName);

                            string guidSuffix = Guid.NewGuid().ToString();

                            string newPhotoName = $"{Path.GetFileNameWithoutExtension(photoName)}_{guidSuffix}{Path.GetExtension(photoName)}";

                            await _blobStorage.CreateFolderIfNotExist(folderPath);

                            string fullPath = $"{folderPath}{newPhotoName}";

                            bool success = await _blobStorage.Upload(fullPath, tenantDTO.TenantLogo.OpenReadStream());

                            if (success)
                            {
                                oldEntity.Logo = fullPath;
                            }
                        }
                        if (!string.IsNullOrEmpty(removeFile)) await _blobStorage.RemoveFile(removeFile);
                    }

                    _context.Tenants.Update(oldEntity);

                    result = await _context.SaveAuthenticationContextAsync(tenantDTO.UserId, DateTimeOffset.Now) > 0;
                }
            }

            if (result)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = true,
                    Message = "Successful",
                    Values = new { Id = tenantDTO.Id }
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

        [HttpGet]
        public async Task<string?> GetTenantStudyLimit(Int64 tenantId)
        {
            return await _context.Tenants.Where(x => x.IsActive && !x.IsDeleted && x.Id == tenantId).Select(x => x.StudyLimit).FirstOrDefaultAsync();
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
        public async Task<bool> SendNewPasswordForUser(Int64 userId)
        {
            var result = false;
            if (userId != 0)
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
            List<Int64> authUserIds = new List<Int64>();

            foreach (string id in authUserIdsArray)
            {
                if (Int64.TryParse(id, out Int64 int64))
                {
                    authUserIds.Add(int64);
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
            if (model.AuthUserId != 0)
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
            if (model.Id == 0)
            {
                if (await _userManager.FindByEmailAsync(model.Email) == null)
                {
                    var firstChar = StringExtensionsHelper.ConvertTRCharToENChar(model.Email).Substring(0, 1).ToLower();
                    var lastNameEng = StringExtensionsHelper.ConvertTRCharToENChar(model.Email).Replace(" ", "").ToLower();
                    var userName = string.Format("{0}_{1}{2}", new Int64(), firstChar, lastNameEng);

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

                    var password = StringExtensionsHelper.GenerateRandomPassword();

                    var userResult = await _userManager.CreateAsync(usr, password);

                    if (userResult.Succeeded)
                    {
                        var addRoleResult = await _userManager.AddToRoleAsync(usr, role?.Name);

                        if (addRoleResult.Succeeded)
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
            if (model.Id != 0)
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
