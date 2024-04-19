using Helios.Authentication.Contexts;
using Helios.Authentication.Helpers;
using Helios.Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Helios.Authentication.Services.Interfaces;
using Helios.Common.DTO;
using Helios.Common.Model;
using MassTransit.Initializers;
using Azure.Storage.Blobs;
using Helios.Common.Enums;
using System.Data;
using Helios.Authentication.Domains.Entities;

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

                    if (model.Role == Roles.StudyUser)
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
                                    await _emailService.SystemAdminUserMail(new SystemAdminDTO
                                    {
                                        Email = user.Email,
                                        Name = user.Name,
                                        LastName = user.LastName,
                                        Password = newPassword

                                    });
                                    return new ApiResponse<dynamic>
                                    {
                                        IsSuccess = true,
                                        Message = "Successful"
                                    };
                                }
                            }
                        }
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

            if (!string.IsNullOrEmpty(model.Password))
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

                var password = StringExtensionsHelper.GenerateRandomPassword();

                var userResult = await _userManager.CreateAsync(usr, password);

                if (userResult.Succeeded)
                {
                    usr.UserRoles.Add(new ApplicationUserRole
                    {
                        Role = role,
                        User = usr,
                        RoleId = role.Id,
                        UserId = usr.Id,
                        StudyId = model.StudyId,
                        TenantId = model.TenantId,
                    });

                    var result = await _context.SaveAuthenticationContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                    if (result)
                    {
                        return new ApiResponse<StudyUserDTO>
                        {
                            IsSuccess = true,
                            Message = "",
                            Values = new StudyUserDTO { AuthUserId = usr.Id, Password = password }
                        };
                    }
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
        public async Task<ApiResponse<DeleteStudyUserDTO>> DeleteStudyUser(StudyUserModel model)
        {
            var aspNetUser = await _userManager.FindByIdAsync(model.AuthUserId.ToString());

            if (aspNetUser != null)
            {
                var aspNetResult = await _userManager.RemoveFromRoleAsync(aspNetUser, Roles.StudyUser.ToString());

                if (aspNetResult.Succeeded)
                {                        
                    return new ApiResponse<DeleteStudyUserDTO>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else
                {
                    return new ApiResponse<DeleteStudyUserDTO>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            else
            {
                return new ApiResponse<DeleteStudyUserDTO>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };
            } 
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
        public async Task<ApiResponse<dynamic>> SetAspNetUser(AspNetUserDTO model)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(model.Id.ToString());
            string oldEmail = user.Email;

            if (user.Email != model.Email)
            {
                var usr = await _userManager.FindByEmailAsync(model.Email);
                if (usr != null)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "The e-mail address you entered is registered. Please try again."
                    };
                }
                user.Email = user.Email != model.Email ? model.Email : user.Email;
                user.ChangePassword = false;
            }

            user.Name = user.Name != model.Name ? model.Name : user.Name;
            user.LastName = user.LastName != model.LastName ? model.LastName : user.LastName;
            user.PhoneNumber = user.PhoneNumber != model.PhoneNumber ? model.PhoneNumber : user.PhoneNumber;

            IdentityResult result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }

            if (oldEmail != model.Email)
            {
                string password = StringExtensionsHelper.GenerateRandomPassword();

                var removeResult = await _userManager.RemovePasswordAsync(user);

                if (removeResult.Succeeded)
                {
                    var passResult = await _userManager.AddPasswordAsync(user, password);
                    if (passResult.Succeeded)
                    {
                        await _emailService.SystemAdminUserMail(new SystemAdminDTO { Name = user.Name, LastName = user.LastName, Email = user.Email, Password = password });
                    }
                }
            }

            return new ApiResponse<dynamic>
            {
                IsSuccess = true,
                Message = "Successful"
            };
        }

        [HttpPost]
        public async Task<ApiResponse<SystemAdminDTO>> SetSystemAdminUser(SystemAdminDTO model)
        {
            if (model.Id == 0)
            {
                var usr = await _userManager.FindByEmailAsync(model.Email);
                var password = "";
                bool isRole = false;

                if (usr == null)
                {
                    var firstChar = StringExtensionsHelper.ConvertTRCharToENChar(model.Name).Substring(0, 1).ToLower();
                    var lastNameEng = StringExtensionsHelper.ConvertTRCharToENChar(model.LastName).Replace(" ", "").ToLower();
                    var userName = string.Format("{0}_{1}{2}", new Int64(), firstChar, lastNameEng);

                    var newUserName = userName;
                    int i = 0;

                    while (await _userManager.Users.AnyAsync(x => x.UserName == newUserName))
                    {
                        i++;
                        newUserName = string.Format("{0}_{1}", userName, i);
                    }

                    var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == Roles.SystemAdmin.ToString());

                    usr = new ApplicationUser
                    {
                        Email = model.Email.Trim(),
                        UserName = newUserName,
                        Name = model.Name.Trim(),
                        LastName = model.LastName.Trim(),
                        PhoneNumber = model.PhoneNumber.Trim(),
                        ChangePassword = false,
                        EmailConfirmed = true,
                        IsActive = true,
                        PhotoBase64String = "",
                        LastChangePasswordDate = DateTime.Now
                    };

                    password = StringExtensionsHelper.GenerateRandomPassword();

                    var userResult = await _userManager.CreateAsync(usr, password);

                    if (userResult.Succeeded)
                    {
                        var addRoleResult = await _userManager.AddToRoleAsync(usr, role?.Name);

                        if (addRoleResult.Succeeded)
                        {
                            isRole = true;
                            model.Password = password;
                            await _emailService.SystemAdminUserMail(model);
                        }
                    }

                    if (string.IsNullOrEmpty(model.Password))
                    {
                        return new ApiResponse<SystemAdminDTO>
                        {
                            IsSuccess = false,
                            Message = "Unsuccessful"
                        };
                    }     
                }
                else
                {
                    var firstChar = StringExtensionsHelper.ConvertTRCharToENChar(model.Name).Substring(0, 1).ToLower();
                    var lastNameEng = StringExtensionsHelper.ConvertTRCharToENChar(model.LastName).Replace(" ", "").ToLower();
                    var userName = string.Format("{0}_{1}{2}", new Int64(), firstChar, lastNameEng);

                    var newUserName = userName;
                    int i = 0;

                    while (await _userManager.Users.AnyAsync(x => x.UserName == newUserName))
                    {
                        i++;
                        newUserName = string.Format("{0}_{1}", userName, i);
                    }

                    usr.Email = model.Email.Trim();
                    usr.UserName = newUserName;
                    usr.Name = model.Name.Trim();
                    usr.LastName = model.LastName.Trim();
                    usr.PhoneNumber = model.PhoneNumber.Trim();
                    usr.ChangePassword = false;
                    usr.EmailConfirmed = true;
                    usr.IsActive = true;
                    usr.PhotoBase64String = "";
                    usr.LastChangePasswordDate = DateTime.Now;

                    var userResult = await _userManager.UpdateAsync(usr);

                    if (!userResult.Succeeded)
                    {
                        return new ApiResponse<SystemAdminDTO>
                        {
                            IsSuccess = false,
                            Message = "Unsuccessful"
                        };
                    }
                }


                if (!await _context.SystemAdmins.AnyAsync(x => x.IsActive && !x.IsDeleted && x.AuthUserId == usr.Id))
                {
                    var result = await AddSystemAdmins(usr, password, isRole);

                    if (result)
                    {
                        return new ApiResponse<SystemAdminDTO>
                        {
                            IsSuccess = true,
                            Message = "Successful",
                            Values = new SystemAdminDTO  { Password = model.Password}
                        };
                    }
                    else
                    {
                        return new ApiResponse<SystemAdminDTO>
                        {
                            IsSuccess = false,
                            Message = "An unexpected error occurred.",
                        };
                    }
                }

          
                return new ApiResponse<SystemAdminDTO>
                {
                    IsSuccess = false,
                    Message = "This user is already registered in the system.",
                };
                
            }
            else
            {
                var user = await _context.SystemAdmins.FirstOrDefaultAsync(x => !x.IsDeleted && x.AuthUserId == model.Id);

                var role = model.RoleIds.FirstOrDefault(x => x == (Int64)Roles.SystemAdmin);

                if (user == null && role != null)
                {
                    var usr = await _userManager.FindByIdAsync(model.Id.ToString());
                    var result = await AddSystemAdmins(usr, "1", false);

                    return new ApiResponse<SystemAdminDTO>
                    {
                        IsSuccess = result,
                        Message = result ? "Successful" : "Unsuccessful"
                    };
                }
                else if (user != null && role == null)
                {
                    var usr = await _userManager.FindByIdAsync(model.Id.ToString());
                    var result = await DeleteSystemAdmin(usr, user);

                    return new ApiResponse<SystemAdminDTO>
                    {
                        IsSuccess = result,
                        Message = result ? "Successful" : "Unsuccessful"
                    };
                }
                else {
                    return new ApiResponse<SystemAdminDTO>
                    {
                        IsSuccess = true,
                        Message = "Successful",
                    };
                }
            }

            async Task<bool> AddSystemAdmins(ApplicationUser? usr, string password, bool isRole)
            {
                try
                {
                    if (string.IsNullOrEmpty(password))
                    {
                        usr.ChangePassword = false;
                        var changePassword = await _userManager.UpdateAsync(usr);
                        if (changePassword.Succeeded)
                        {
                            string newPassword = StringExtensionsHelper.GenerateRandomPassword();
                            var removeResult = await _userManager.RemovePasswordAsync(usr);
                            if (removeResult.Succeeded)
                            {
                                var passResult = await _userManager.AddPasswordAsync(usr, newPassword);
                                if (passResult.Succeeded)
                                {
                                    model.Password = newPassword;
                                    await _emailService.SystemAdminUserMail(model);
                                }
                            }
                        }
                    }


                    if (!isRole)
                    {
                        var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == Roles.SystemAdmin.ToString());

                        var addRoleResult = await _userManager.AddToRoleAsync(usr, role?.Name);

                        if (!addRoleResult.Succeeded)
                        {
                            return false;
                        }
                    }

                    await _context.SystemAdmins.AddAsync(new SystemAdmin
                    {
                        AuthUserId = usr.Id,
                    });

                    return await _context.SaveAuthenticationContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            async Task<bool> DeleteSystemAdmin(ApplicationUser? usr, SystemAdmin? systemAdmin)
            {
                try
                {
                    var aspNetResult = await _userManager.RemoveFromRoleAsync(usr, Roles.SystemAdmin.ToString());

                    if (aspNetResult.Succeeded)
                    {
                        _context.SystemAdmins.Remove(systemAdmin);

                        var result = await _context.SaveAuthenticationContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                        if (result)
                        {
                            var userRoles = await _userManager.GetRolesAsync(usr);

                            if (!userRoles.Any())
                            {
                                usr.IsActive = false;
                                usr.ChangePassword = false;
                                var resultAspNetUser = await _userManager.UpdateAsync(usr);

                                if (!resultAspNetUser.Succeeded)
                                {
                                    return false;
                                }
                            }

                            return true;
                        }
                    }

                   return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
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
                    Name = userManagerUser.Name,
                    LastName = userManagerUser.LastName,
                    Email = userManagerUser.Email,
                    PhoneNumber = userManagerUser.PhoneNumber,
                    IsActive = systemAdmin.IsActive
                }
            ).ToListAsync();
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SystemAdminActivePassive(SystemAdminDTO model)
        {
            var user = await _context.SystemAdmins.FirstOrDefaultAsync(x => !x.IsDeleted && x.AuthUserId == model.Id);

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
            var user = await _context.SystemAdmins.FirstOrDefaultAsync(x => x.AuthUserId == model.Id && !x.IsDeleted);

            if (user != null)
            {
                var aspNetUser = await _userManager.FindByIdAsync(user.AuthUserId.ToString());

                if (aspNetUser != null)
                {
                    var aspNetResult = await _userManager.RemoveFromRoleAsync(aspNetUser, Roles.SystemAdmin.ToString());

                    if (aspNetResult.Succeeded)
                    {
                        _context.SystemAdmins.Remove(user);

                        var result = await _context.SaveAuthenticationContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                        if (result)
                        {
                            var userRoles = await _userManager.GetRolesAsync(aspNetUser);

                            if (!userRoles.Any())
                            {
                                aspNetUser.IsActive = false;
                                var resultAspNetUser = await _userManager.UpdateAsync(aspNetUser);

                                if (!resultAspNetUser.Succeeded)
                                {
                                    return new ApiResponse<dynamic>
                                    {
                                        IsSuccess = false,
                                        Message = "Unsuccessful"
                                    };
                                }
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
                                Message = "Unsuccessful"
                            };
                        }
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
            }

            return new ApiResponse<dynamic>
            {
                IsSuccess = false,
                Message = "An unexpected error occurred."
            };
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetTenantAdminUser(SystemAdminDTO model)
        {
            if (model.Id == 0)
            {
                var usr = await _userManager.FindByEmailAsync(model.Email);
                bool isRole = false;

                if (usr == null)
                {
                    var firstChar = StringExtensionsHelper.ConvertTRCharToENChar(model.Name).Substring(0, 1).ToLower();
                    var lastNameEng = StringExtensionsHelper.ConvertTRCharToENChar(model.LastName).Replace(" ", "").ToLower();
                    var userName = string.Format("{0}_{1}{2}", new Int64(), firstChar, lastNameEng);

                    var newUserName = userName;
                    int i = 0;

                    while (await _userManager.Users.AnyAsync(x => x.UserName == newUserName))
                    {
                        i++;
                        newUserName = string.Format("{0}_{1}", userName, i);
                    }

                    var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == Roles.TenantAdmin.ToString());

                    usr = new ApplicationUser
                    {
                        Email = model.Email.Trim(),
                        UserName = newUserName,
                        Name = model.Name.Trim(),
                        LastName = model.LastName.Trim(),
                        PhoneNumber = model.PhoneNumber.Trim(),
                        ChangePassword = false,
                        EmailConfirmed = true,
                        IsActive = true,
                        PhotoBase64String = "",
                        LastChangePasswordDate = DateTime.Now
                    };

                    model.Password = StringExtensionsHelper.GenerateRandomPassword();

                    var userResult = await _userManager.CreateAsync(usr, model.Password);

                    if (userResult.Succeeded)
                    {
                        var addRoleResult = await _userManager.AddToRoleAsync(usr, role?.Name);

                        if (addRoleResult.Succeeded)
                        {
                            isRole = true;
                            await _emailService.SystemAdminUserMail(model);
                        }
                    }

                    if (string.IsNullOrEmpty(model.Password))
                    {
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = false,
                            Message = "Unsuccessful"
                        };
                    }
                }
                else if(string.IsNullOrEmpty(model.Password))
                {
                    var firstChar = StringExtensionsHelper.ConvertTRCharToENChar(model.Name).Substring(0, 1).ToLower();
                    var lastNameEng = StringExtensionsHelper.ConvertTRCharToENChar(model.LastName).Replace(" ", "").ToLower();
                    var userName = string.Format("{0}_{1}{2}", new Int64(), firstChar, lastNameEng);

                    var newUserName = userName;
                    int i = 0;

                    while (await _userManager.Users.AnyAsync(x => x.UserName == newUserName))
                    {
                        i++;
                        newUserName = string.Format("{0}_{1}", userName, i);
                    }

                    usr.Email = model.Email.Trim();
                    usr.UserName = newUserName;
                    usr.Name = model.Name.Trim();
                    usr.LastName = model.LastName.Trim();
                    usr.PhoneNumber = model.PhoneNumber.Trim();
                    usr.ChangePassword = false;
                    usr.EmailConfirmed = true;
                    usr.IsActive = true;
                    usr.PhotoBase64String = "";
                    usr.LastChangePasswordDate = DateTime.Now;

                    var userResult = await _userManager.UpdateAsync(usr);

                    if (!userResult.Succeeded)
                    {
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = false,
                            Message = "Unsuccessful"
                        };
                    }
                }

                if (!await _context.TenantAdmins.AnyAsync(x => x.IsActive && !x.IsDeleted && x.AuthUserId == usr.Id))
                {
                    var result = await AddTenantAdmins(usr, model.Password, isRole);

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
                            Message = "An unexpected error occurred.",
                        };
                    }
                }

                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "This user is already registered in the system.",
                };
            }
            else
            {
                var user = await _context.TenantAdmins.Where(x => !x.IsDeleted && x.AuthUserId == model.Id).ToListAsync();

                var areEqual = AreArraysEqual(user.Select(x => x.TenantId), model.TenantIds);

                if (areEqual)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }

                foreach (var userItem in user)
                {
                    if (!model.TenantIds.Contains(userItem.TenantId))
                    {
                        _context.TenantAdmins.Remove(userItem);
                    }
                }

                foreach (var tenantId in model.TenantIds)
                {
                    if (!user.Any(x => x.TenantId == tenantId))
                    {
                        var newUser = new TenantAdmin { AuthUserId = model.Id, TenantId = tenantId };
                        _context.TenantAdmins.Add(newUser);
                    }
                }

                var result = await _context.SaveAuthenticationContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                if (result)
                {
                    user = await _context.TenantAdmins.Where(x => !x.IsDeleted && x.AuthUserId == model.Id).ToListAsync();
                    var usr = await _userManager.FindByIdAsync(model.Id.ToString());

                    if (user.Count == 0)
                    {
                        var aspNetResult = await _userManager.RemoveFromRoleAsync(usr, Roles.TenantAdmin.ToString());

                        if (aspNetResult.Succeeded)
                        {
                            var userRoles = await _userManager.GetRolesAsync(usr);

                            if (!userRoles.Any())
                            {
                                usr.IsActive = false;
                                usr.ChangePassword = false;
                                var resultAspNetUser = await _userManager.UpdateAsync(usr);

                                if (!resultAspNetUser.Succeeded)
                                {
                                    return new ApiResponse<dynamic>
                                    {
                                        IsSuccess = false,
                                        Message = "Unsuccessful"
                                    };
                                }
                            }
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
                        bool isTenantAdmin = await _userManager.IsInRoleAsync(usr, Roles.TenantAdmin.ToString());

                        if (!isTenantAdmin)
                        {
                            var r = await _userManager.AddToRoleAsync(usr, Roles.TenantAdmin.ToString());

                            if (!r.Succeeded) {
                                return new ApiResponse<dynamic>
                                {
                                    IsSuccess = false,
                                    Message = "Unsuccessful"
                                };
                            }
                        }
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
                Message = "This user is already registered in the system.",
            };


            async Task<bool> AddTenantAdmins(ApplicationUser? usr, string password, bool isRole)
            {
                try
                {
                    if (string.IsNullOrEmpty(password))
                    {
                        usr.ChangePassword = false;
                        var changePassword = await _userManager.UpdateAsync(usr);
                        if (changePassword.Succeeded)
                        {
                            string newPassword = StringExtensionsHelper.GenerateRandomPassword();
                            var removeResult = await _userManager.RemovePasswordAsync(usr);
                            if (removeResult.Succeeded)
                            {
                                var passResult = await _userManager.AddPasswordAsync(usr, newPassword);
                                if (passResult.Succeeded)
                                {
                                    model.Password = newPassword;
                                    await _emailService.SystemAdminUserMail(model);
                                }
                            }
                        }
                    }

                    if (!isRole)
                    {
                        var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == Roles.TenantAdmin.ToString());

                        var addRoleResult = await _userManager.AddToRoleAsync(usr, role?.Name);

                        if (!addRoleResult.Succeeded)
                        {
                            return false;
                        }
                    }

                    List<TenantAdmin> tenantAdmins = model.TenantIds.Select(x => new TenantAdmin
                    {
                        AuthUserId = usr.Id,
                        TenantId = x
                    }).ToList();

                    if (tenantAdmins.Count > 0)
                    {
                        await _context.TenantAdmins.AddRangeAsync(tenantAdmins);
                    }

                    return await _context.SaveAuthenticationContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                }
                catch (Exception)
                {
                    return false;
                }              
            }

            bool AreArraysEqual<T>(IEnumerable<T> array1, IEnumerable<T> array2)
            {
                if (array1 == null && array2 == null)
                {
                    return true;
                }

                if (array1 == null || array2 == null)
                {
                    return false;
                }

                return array1.OrderBy(x => x).SequenceEqual(array2.OrderBy(x => x));
            }
        }

        [HttpGet]
        public async Task<List<SystemUserModel>> GetTenantAndSystemAdminUserList(Int64 id)
        {
            List<Tenant> emptyTenantList = new List<Tenant>();

            var result = await (
                from userManagerUser in _userManager.Users
                join systemAdmin in _context.SystemAdmins on userManagerUser.Id equals systemAdmin.AuthUserId into systemAdmins
                from sysAdmin in systemAdmins.DefaultIfEmpty()
                join tenantAdmin in _context.TenantAdmins on userManagerUser.Id equals tenantAdmin.AuthUserId into tenantAdmins
                from tenAdmin in tenantAdmins.DefaultIfEmpty()
                join tenant in _context.Tenants on tenAdmin.TenantId equals tenant.Id into tenants
                where
                    userManagerUser.IsActive &&
                    userManagerUser.Id != id &&
                    (sysAdmin != null && !sysAdmin.IsDeleted) ||
                    (tenAdmin != null && !tenAdmin.IsDeleted)
                select new SystemUserModel
                {
                    Id = userManagerUser.Id,
                    Name = userManagerUser.Name,
                    LastName = userManagerUser.LastName,
                    Email = userManagerUser.Email,
                    PhoneNumber = userManagerUser.PhoneNumber,
                    IsActive = (sysAdmin != null && sysAdmin.IsActive) || (tenAdmin != null && tenAdmin.IsActive) || userManagerUser.IsActive,
                    Roles = userManagerUser.UserRoles.Select(ur => new
                    {
                        RoleId = ur.RoleId,
                        RoleName = ur.Role.Name
                    }).ToList(),
                    Tenants = tenAdmin != null && !tenAdmin.IsDeleted ? 
                        tenants.Where(t => !t.IsDeleted)
                                .Select(t => new
                                {
                                    TenantId = t.Id,
                                    TenantName = t.Name
                                }).ToList()
                    : emptyTenantList
                }
            ).ToListAsync();

            var distinctResult = result
                .GroupBy(x => x.Id)
                .Select(group => new SystemUserModel
                {
                    Id = group.Key,
                    Name = group.First().Name,
                    LastName = group.First().LastName,
                    Email = group.First().Email,
                    PhoneNumber = group.First().PhoneNumber,
                    IsActive = group.First().IsActive,
                    Roles = group.First().Roles,
                    Tenants = group.SelectMany(x => x.Tenants)
                      .GroupBy(t => t.TenantId)
                      .Select(tGroup => new Tenant
                      {
                          Id = tGroup.First().TenantId,
                          Name = tGroup.First().TenantName
                      })
                      .ToList()
                })
                .ToList();

            return distinctResult;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> TenantAdminDelete(TenantAndSystemAdminDTO model)
        {
            var user = await _context.TenantAdmins.Where(x => !x.IsDeleted && x.AuthUserId == model.Id).ToListAsync();

            if (user.Count > 0)
            {
                _context.TenantAdmins.RemoveRange(user.Where(x=>model.TenantIds.Contains(x.TenantId)));

                var result = await _context.SaveAuthenticationContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                if (result)
                {
                    if (!user.Any(x => !model.TenantIds.Contains(x.TenantId)))
                    {
                        var aspNetUser = await _userManager.FindByIdAsync(user.First().AuthUserId.ToString());

                        if (aspNetUser != null)
                        {
                            var aspNetResult = await _userManager.RemoveFromRoleAsync(aspNetUser, Roles.TenantAdmin.ToString());

                            if (!aspNetResult.Succeeded)
                            {
                                return new ApiResponse<dynamic>
                                {
                                    IsSuccess = true,
                                    Message = "Unsuccessful"
                                };
                            }

                            var userRoles = await _userManager.GetRolesAsync(aspNetUser);

                            if (!userRoles.Any())
                            {
                                aspNetUser.IsActive = false;
                                var resultAspNetUser = await _userManager.UpdateAsync(aspNetUser);

                                if (!resultAspNetUser.Succeeded)
                                {
                                    return new ApiResponse<dynamic>
                                    {
                                        IsSuccess = false,
                                        Message = "Unsuccessful"
                                    };
                                }
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
        public async Task<ApiResponse<dynamic>> TenantAdminActivePassive(TenantAndSystemAdminDTO model)
        {
            var user = await _context.TenantAdmins.Where(x => !x.IsDeleted && x.AuthUserId == model.Id && model.TenantIds.Contains(x.TenantId)).ToListAsync();

            if (user.Count > 0)
            {
                user.ForEach(x => x.IsActive = !x.IsActive);

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
