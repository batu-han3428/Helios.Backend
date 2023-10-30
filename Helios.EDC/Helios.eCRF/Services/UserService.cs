using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.Core.Domains.Entities;
using Helios.eCRF.Models;
using Helios.eCRF.Services.Base;
using Helios.eCRF.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<bool> AddTenant(TenantModel model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/AddTenant", Method.Post);
                //req.AddHeader("Name", name);
                //req.AddParameter("Name", name);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
                return result.IsSuccessful;
            }
        }

        public async Task<bool> UpdateTenant(TenantModel model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/UpdateTenant", Method.Post);
                //req.AddHeader("Name", name);
                //req.AddParameter("Name", name);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
                return result.IsSuccessful;
            }
        }

        public async Task<TenantModel> GetTenant(Guid id)
        {
            var tenant = new TenantModel();

            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/GetTenant", Method.Get);
                req.AddParameter("id", id);
                var result = await client.ExecuteAsync(req);
                tenant = JsonConvert.DeserializeObject<TenantModel>(result.Content);
            }

            return tenant;
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

        public async Task<bool> UpdateUser(UserDTO model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/UpdateUser", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
            }

            return false;
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

        public async Task<List<TenantModel>> GetTenantList()
        {
            var tenantList = new List<TenantModel>();

            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/GetTenantList", Method.Get);
                var result = await client.ExecuteAsync(req);
                tenantList = JsonConvert.DeserializeObject<List<TenantModel>>(result.Content);
            }

            return tenantList;
        }

        #region Permissions
        public async Task<List<UserPermissionDTO>> GetPermissionRoleList(Guid studyId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/GetPermissionRoleList", Method.Get);
                req.AddParameter("studyId", studyId);
                var result = await client.ExecuteAsync<List<UserPermissionDTO>>(req);
                return result.Data;
            }
        }

        public async Task<List<UserPermissionDTO>> GetRoleList(Guid studyId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/GetRoleList", Method.Get);
                req.AddParameter("studyId", studyId);
                var result = await client.ExecuteAsync<List<UserPermissionDTO>>(req);
                return result.Data;
            }
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

        private async Task<List<StudyUserDTO>> GetStudyUsers(Guid studyId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/GetStudyUsers", Method.Get);
                req.AddParameter("studyId", studyId);
                var result = await client.ExecuteAsync<List<StudyUserDTO>>(req);
                return result.Data;
            }
        }

        private async Task<List<AspNetUserDTO>> GetUserList(List<Guid> AuthUserIds)
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

        private async Task<bool> GetCheckStudyUser(Guid authUserId, Guid studyId)
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

        public async Task<List<StudyUserDTO>> GetStudyUserList(Guid studyId)
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
                                        CreatedOn = studyUser.CreatedOn,
                                        LastUpdatedOn = studyUser.LastUpdatedOn
                                    }).ToList();
                }
            }

            return new List<StudyUserDTO>();
        }

        public async Task<ApiResponse<StudyUserDTO>> GetStudyUser(string email, Guid studyId)
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
            if (studyUserModel.AuthUserId == Guid.Empty)
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

            if (response.IsSuccess && studyUserModel.StudyUserId == Guid.Empty)
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
    }
}
