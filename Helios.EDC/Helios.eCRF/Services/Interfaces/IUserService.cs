using Helios.eCRF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Helios.Common.DTO;
using Helios.Common.Model;

namespace Helios.eCRF.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> AddTenant(TenantModel model);
        Task<bool> UpdateTenant(TenantModel model);
        Task<TenantModel> GetTenant(Guid id);
        Task<UserDTO> GetUserByEmail(string mail);
        Task<dynamic> AddUser(UserDTO model);
        Task<bool> PassiveOrActiveUser(UserDTO model);
        Task<ApiResponse<dynamic>> UpdateUser(AspNetUserDTO model);
        Task<bool> AddRole(UserDTO model);
        Task<List<TenantModel>> GetTenantList();
        Task<ApiResponse<dynamic>> AddOrUpdatePermissionRol(UserPermissionModel userPermission);
        Task<ApiResponse<dynamic>> DeleteRole(UserPermissionModel userPermission);
        Task<List<UserPermissionDTO>> GetPermissionRoleList(Guid studyId);
        Task<ApiResponse<dynamic>> SetPermission(SetPermissionModel setPermissionModel);
        Task<List<StudyUserDTO>> GetStudyUserList(Guid studyId);
        Task<ApiResponse<dynamic>> SetStudyUser(StudyUserModel studyUserModel);
        Task<ApiResponse<dynamic>> ActivePassiveStudyUser(StudyUserModel studyUserModel);
        Task<ApiResponse<dynamic>> DeleteStudyUser(StudyUserModel studyUserModel);
        Task<ApiResponse<dynamic>> UserResetPassword(StudyUserModel model);
        Task<ApiResponse<StudyUserDTO>> GetStudyUser(string email, Guid studyId);
        Task<List<UserPermissionDTO>> GetRoleList(Guid studyId);
        Task<List<TenantUserDTO>> GetTenantUserList(Guid tenantId);
        Task<ApiResponse<dynamic>> SetTenantUser(TenantUserModel studyUserModel);
    }
}
