using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.eCRF.Models;
using Helios.eCRF.Services.Base;
using Helios.eCRF.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;

namespace Helios.eCRF.Services
{
    public class UserService : ApiBaseService, IUserService
    {
        public UserService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<UserDTO> GetUserByEmail(string mail)
        {
            var user = new UserDTO();

            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/GetUserByEmail", Method.Get);
                req.AddParameter("mail", mail);
                var result = await client.ExecuteAsync(req);
                user = JsonConvert.DeserializeObject<UserDTO>(result.Content);
            }

            return user;
        }

        public async Task<dynamic> AddUser(UserDTO model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/AddUser", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync<dynamic>(req);
                return result.Data;
            }
        }

        public async Task<bool> PassiveOrActiveUser(UserDTO model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/PassiveOrActiveUser", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
            }

            return false;
        }

        public async Task<ApiResponse<dynamic>> UpdateUser(AspNetUserDTO model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/UpdateUser", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }

        public async Task<bool> AddRole(UserDTO model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/AddRole", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
            }

            return false;
        }

        #region Tenants
        private async Task<List<TenantModel>> GetAuthTenantList()
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/GetTenantList", Method.Get);
                var result = await client.ExecuteAsync<List<TenantModel>>(req);
                return result.Data;
            }
        }

        private async Task<List<TenantModel>> GetTenantsStudyCount(List<Int64> tenantIds)
        {
            using (var client = CoreServiceClient)
            {
                string tenantIdsString = string.Join(",", tenantIds);
                var req = new RestRequest("CoreUser/GetTenantsStudyCount", Method.Get);
                req.AddParameter("tenantIds", tenantIdsString);
                var result = await client.ExecuteAsync<List<TenantModel>>(req);
                return result.Data;
            }
        }

        public async Task<List<TenantModel>> GetTenantList()
        {
            var authTenants = await GetAuthTenantList();

            if (authTenants != null && authTenants.Count > 0)
            {
                var counts = await GetTenantsStudyCount(authTenants.Select(x=>x.Id).ToList());

                if (counts != null && counts.Count > 0)
                {
                    return (from aTenants in authTenants
                                 join count in counts on aTenants.Id equals count.Id into countGroup
                                 from count in countGroup.DefaultIfEmpty()
                                 select new TenantModel
                                 {
                                     Id = aTenants.Id,
                                     Name = aTenants.Name,
                                     ActiveStudies = (count != null) ? count.ActiveStudies + " / " + aTenants.StudyLimit : "0 / " + (aTenants.StudyLimit != null ? aTenants.StudyLimit : "0"),
                                     CreatedAt = aTenants.CreatedAt,
                                     UpdatedAt = aTenants.UpdatedAt
                                 }).ToList();
                }
                else
                {
                    return authTenants;
                }
            }

            return new List<TenantModel>();
        }

        public async Task<TenantModel> GetTenant(Int64 tenantId)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/GetTenant", Method.Get);
                req.AddParameter("tenantId", tenantId);
                var result = await client.ExecuteAsync<TenantModel>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> SetTenant(TenantDTO tenantDTO)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/SetTenant", Method.Post);
                req.AddParameter("Id", tenantDTO.Id?.ToString());
                req.AddParameter("UserId", tenantDTO.UserId);
                req.AddParameter("TenantName", tenantDTO.TenantName);
                req.AddParameter("TimeZone", tenantDTO.TimeZone);
                req.AddParameter("StudyLimit", tenantDTO.StudyLimit);
                req.AddParameter("UserLimit", tenantDTO.UserLimit);
                if (tenantDTO.TenantLogo != null && tenantDTO.TenantLogo.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        tenantDTO.TenantLogo.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        req.AddFile("TenantLogo", fileBytes, tenantDTO.TenantLogo.FileName, tenantDTO.TenantLogo.ContentType);
                    }
                }
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }
        #endregion

        #region Permissions
        public async Task<List<UserPermissionDTO>> GetPermissionRoleList(Int64 studyId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/GetPermissionRoleList", Method.Get);
                req.AddParameter("studyId", studyId);
                var result = await client.ExecuteAsync<List<UserPermissionDTO>>(req);
                return result.Data;
            }
        }

        public async Task<List<UserPermissionDTO>> GetRoleList(Int64 studyId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/GetRoleList", Method.Get);
                req.AddParameter("studyId", studyId);
                var result = await client.ExecuteAsync<List<UserPermissionDTO>>(req);
                return result.Data;
            }
        }

        private async Task<UserPermissionRoleDTO> GetUserIdsRole(Int64 roleId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/GetUserIdsRole", Method.Get);
                req.AddParameter("roleId", roleId);
                var result = await client.ExecuteAsync<UserPermissionRoleDTO>(req);
                return result.Data;
            }
        }
        
        public async Task<List<UserPermissionRoleModel>> GetRoleUsers(Int64 roleId)
        {
            var userIdsRole = await GetUserIdsRole(roleId);

            if (userIdsRole != null && userIdsRole.UserIds.Count > 0)
            {
                var result = await GetUserList(userIdsRole.UserIds);

                return result.Select(x => new UserPermissionRoleModel
                {
                    Role = userIdsRole.Role,
                    Name = x.Name + " " + x.LastName
                }).ToList();
            }

            return new List<UserPermissionRoleModel>();
        }

        private async Task<List<StudyUsersRolesDTO>> GetStudyUserIdsRole(Int64 studyId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/GetStudyUserIdsRole", Method.Get);
                req.AddParameter("studyId", studyId);
                var result = await client.ExecuteAsync<List<StudyUsersRolesDTO>>(req);
                return result.Data;
            }
        }

        public async Task<List<UserPermissionRoleModel>> GetStudyRoleUsers(Int64 studyId)
        {
            var userIdsRole = await GetStudyUserIdsRole(studyId);

            if (userIdsRole.Count > 0)
            {
                var result = await GetUserList(userIdsRole.Select(x=>x.Id).ToList());

                return result.Join(userIdsRole, aspNetUser => aspNetUser.Id, studyUser => studyUser.Id, (aspNetUser, studyUser) =>
                                    new UserPermissionRoleModel
                                    {
                                        Role = studyUser.RoleName,
                                        Name = aspNetUser.Name + " " + aspNetUser.LastName
                                    }).ToList();
            }

            return new List<UserPermissionRoleModel>();
        }

        public async Task<ApiResponse<dynamic>> SetPermission(SetPermissionModel setPermissionModel)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/SetPermission", Method.Post);
                req.AddJsonBody(setPermissionModel);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> AddOrUpdatePermissionRol(UserPermissionModel userPermission)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/AddOrUpdatePermissionRol", Method.Post);
                req.AddJsonBody(userPermission);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> DeleteRole(UserPermissionModel userPermission)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/DeleteRole", Method.Post);
                req.AddJsonBody(userPermission);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }
        #endregion

        #region Study user

        private async Task<List<StudyUserDTO>> GetStudyUsers(Int64 studyId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/GetStudyUsers", Method.Get);
                req.AddParameter("studyId", studyId);
                var result = await client.ExecuteAsync<List<StudyUserDTO>>(req);
                return result.Data;
            }
        }

        private async Task<List<AspNetUserDTO>> GetUserList(List<Int64> AuthUserIds)
        {
            using (var client = AuthServiceClient)
            {
                string authUserIdsString = string.Join(",", AuthUserIds);
                var req = new RestRequest("AdminUser/GetUserList", Method.Get);
                req.AddParameter("AuthUserIds", authUserIdsString);
                var users = await client.ExecuteAsync<List<AspNetUserDTO>>(req);
                return users.Data;
            }
        }

        private async Task<AspNetUserDTO> GetUser(string email)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/GetUser", Method.Get);
                req.AddParameter("email", email);
                var result = await client.ExecuteAsync<AspNetUserDTO>(req);
                return result.Data;
            }
        }

        private async Task<bool> GetCheckStudyUser(Int64 authUserId, Int64 studyId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/GetCheckStudyUser", Method.Get);
                req.AddParameter("authUserId", authUserId);
                req.AddParameter("studyId", studyId);
                var result = await client.ExecuteAsync<bool>(req);
                return result.Data;
            }
        }

        private async Task<ApiResponse<StudyUserDTO>> AddStudyUser(StudyUserModel model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/AddStudyUser", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync<ApiResponse<StudyUserDTO>>(req);
                return result.Data;
            }
        }

        private async Task<ApiResponse<dynamic>> SetStudyUserCore(StudyUserModel studyUserModel)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/SetStudyUser", Method.Post);
                req.AddJsonBody(studyUserModel);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }

        private async Task AddStudyUserMail(StudyUserModel model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/AddStudyUserMail", Method.Post);
                req.AddJsonBody(model);
                await client.ExecuteAsync(req);
            }
        }

        public async Task<List<StudyUserDTO>> GetStudyUserList(Int64 studyId)
        {
            List<StudyUserDTO> studyUsers = await GetStudyUsers(studyId);

            if (studyUsers.Count > 0)
            {
                List<AspNetUserDTO> aspNetUserDTOs = await GetUserList(studyUsers.Select(x => x.AuthUserId).ToList());

                if (aspNetUserDTOs.Count > 0)
                {
                    return aspNetUserDTOs.Join(studyUsers, aspNetUser => aspNetUser.Id, studyUser => studyUser.AuthUserId, (aspNetUser, studyUser) =>
                                    new StudyUserDTO
                                    {
                                        StudyUserId = studyUser.StudyUserId,
                                        AuthUserId = aspNetUser.Id,
                                        Name = aspNetUser.Name,
                                        LastName = aspNetUser.LastName,
                                        IsActive = studyUser.IsActive,
                                        Email = aspNetUser.Email,
                                        RoleName = studyUser.RoleName,
                                        RoleId = studyUser.RoleId,
                                        Sites = studyUser.Sites,
                                        ResponsiblePerson = studyUser.ResponsiblePerson,
                                        CreatedOn = studyUser.CreatedOn,
                                        LastUpdatedOn = studyUser.LastUpdatedOn
                                    }).ToList();
                }
            }

            return new List<StudyUserDTO>();
        }

        public async Task<ApiResponse<StudyUserDTO>> GetStudyUser(string email, Int64 studyId)
        {
            var user = await GetUser(email);

            if (user != null)
            {
                if (GetCheckStudyUser(user.Id, studyId).Result)
                {
                    return new ApiResponse<StudyUserDTO>
                    {
                        IsSuccess = false,
                        Message = "This user is already registered in the system."
                    };
                }
                else
                {
                    return new ApiResponse<StudyUserDTO>
                    {
                        IsSuccess = true,
                        Message = "",
                        Values = new StudyUserDTO
                        {
                            AuthUserId = user.Id,
                            Name = user.Name,
                            LastName = user.LastName,
                            Email = user.Email
                        }
                    };
                }
            }
            else
            {
                return new ApiResponse<StudyUserDTO>
                {
                    IsSuccess = true,
                    Message = "",
                    Values = new StudyUserDTO()
                };
            }
        }

        public async Task<ApiResponse<dynamic>> SetStudyUser(StudyUserModel studyUserModel)
        {
            if (studyUserModel.AuthUserId == 0)
            {
                var result = await AddStudyUser(studyUserModel);

                if (result.IsSuccess)
                {
                    studyUserModel.AuthUserId = result.Values.AuthUserId;
                    studyUserModel.Password = result.Values.Password;
                    studyUserModel.FirstAddition = true;
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

            var response = await SetStudyUserCore(studyUserModel);

            if (response.IsSuccess && studyUserModel.StudyUserId == 0)
            {
                await AddStudyUserMail(studyUserModel);
            }

            return response;
        }

        public async Task<ApiResponse<dynamic>> ActivePassiveStudyUser(StudyUserModel studyUserModel)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/ActivePassiveStudyUser", Method.Post);
                req.AddJsonBody(studyUserModel);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> ActivePassiveStudyUsers(StudyUserModel studyUserModel)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/ActivePassiveStudyUsers", Method.Post);
                req.AddJsonBody(studyUserModel);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> DeleteStudyUser(StudyUserModel studyUserModel)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/DeleteStudyUser", Method.Post);
                req.AddJsonBody(studyUserModel);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> UserResetPassword(StudyUserModel model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/UserResetPassword", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }
        #endregion

        #region Tenant user

        private async Task<List<TenantUserDTO>> GetTenantUsers(Int64 tenantId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/GetTenantUsers", Method.Get);
                req.AddParameter("tenantId", tenantId);
                var result = await client.ExecuteAsync<List<TenantUserDTO>>(req);
                return result.Data;
            }
        }

        public async Task<List<TenantUserDTO>> GetTenantUserList(Int64 tenantId)
        {
            List<TenantUserDTO> tenantUsers = await GetTenantUsers(tenantId);

            if (tenantUsers.Count > 0)
            {
                List<AspNetUserDTO> aspNetUserDTOs = await GetUserList(tenantUsers.Select(x => x.AuthUserId).ToList());

                if (aspNetUserDTOs.Count > 0)
                {
                    return aspNetUserDTOs.Join(tenantUsers, aspNetUser => aspNetUser.Id, tenantUser => tenantUser.AuthUserId, (aspNetUser, tenantUser) =>
                                    new TenantUserDTO
                                    {
                                        StudyUserId = tenantUser.StudyUserId,
                                        AuthUserId = aspNetUser.Id,
                                        StudyId = tenantUser.StudyId,
                                        Name = aspNetUser.Name,
                                        LastName = aspNetUser.LastName,
                                        IsActive = tenantUser.IsActive,
                                        Email = aspNetUser.Email,
                                        StudyName = tenantUser.StudyName,
                                        StudyDemoLive = tenantUser.StudyDemoLive,
                                        CreatedOn = tenantUser.CreatedOn,
                                        LastUpdatedOn = tenantUser.LastUpdatedOn
                                    }).ToList();
                }
            }

            return new List<TenantUserDTO>();
        }

        public async Task<ApiResponse<dynamic>> SetTenantUser(TenantUserModel tenantUserModel)
        {
            return await UpdateUser(new AspNetUserDTO
            {
                Id = tenantUserModel.AuthUserId,
                Email = tenantUserModel.Email,
                Name = tenantUserModel.Name,
                LastName = tenantUserModel.LastName
            });
        }
 
        #endregion

        #region System Admin User

        public async Task<ApiResponse<dynamic>> SetSystemAdminUser(SystemAdminDTO systemAdminDTO)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/SetSystemAdminUser", Method.Post);
                req.AddJsonBody(systemAdminDTO);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }

        public async Task<List<SystemUserModel>> GetSystemAdminUserList()
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/GetSystemAdminUserList", Method.Get);
                var result = await client.ExecuteAsync<List<SystemUserModel>>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> SystemAdminActivePassive(SystemAdminDTO systemAdminDTO)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/SystemAdminActivePassive", Method.Post);
                req.AddJsonBody(systemAdminDTO);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> SystemAdminResetPassword(SystemAdminDTO systemAdminDTO)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/SystemAdminResetPassword", Method.Post);
                req.AddJsonBody(systemAdminDTO);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> SystemAdminDelete(SystemAdminDTO systemAdminDTO)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/SystemAdminDelete", Method.Post);
                req.AddJsonBody(systemAdminDTO);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }
        #endregion

        #region SSO
        public async Task<List<TenantUserModel>> GetUserTenantList(Int64 userId)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AuthAccount/GetUserTenantList", Method.Get);
                req.AddParameter("userId", userId);
                var result = await client.ExecuteAsync<List<TenantUserModel>>(req);
                return result.Data;
            }
        }

        public async Task<List<SSOUserStudyModel>> GetUserStudiesList(Int64 tenantId, Int64 userId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/GetUserStudiesList", Method.Get);
                req.AddParameter("tenantId", tenantId);
                req.AddParameter("userId", userId);
                var result = await client.ExecuteAsync<List<SSOUserStudyModel>>(req);
                return result.Data;
            }
        }
        #endregion
    }
}
