﻿using Helios.eCRF.Models;
using Helios.Common.DTO;
using Helios.Common.Model;

namespace Helios.eCRF.Services.Interfaces
{
    public interface IUserService
    {
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
        Task<ApiResponse<dynamic>> ActivePassiveStudyUsers(StudyUserModel studyUserModel);
        Task<ApiResponse<dynamic>> DeleteStudyUser(StudyUserModel studyUserModel);
        Task<ApiResponse<dynamic>> UserResetPassword(StudyUserModel model);
        Task<ApiResponse<StudyUserDTO>> GetStudyUser(string email, Guid studyId);
        Task<List<UserPermissionDTO>> GetRoleList(Guid studyId);
        Task<List<TenantUserDTO>> GetTenantUserList(Guid tenantId);
        Task<ApiResponse<dynamic>> SetTenantUser(TenantUserModel studyUserModel);
        Task<List<TenantUserModel>> GetUserTenantList(Guid userId);
        Task<List<SSOUserStudyModel>> GetUserStudiesList(Guid tenantId, Guid userId);
        Task<List<UserPermissionRoleModel>> GetRoleUsers(Guid roleId);
        Task<List<UserPermissionRoleModel>> GetStudyRoleUsers(Guid studyId);
        Task<ApiResponse<dynamic>> SetSystemAdminUser(SystemAdminDTO systemAdminDTO);
        Task<List<SystemUserModel>> GetSystemAdminUserList();
        Task<ApiResponse<dynamic>> SystemAdminActivePassive(SystemAdminDTO systemAdminDTO);
        Task<ApiResponse<dynamic>> SystemAdminResetPassword(SystemAdminDTO systemAdminDTO);
        Task<ApiResponse<dynamic>> SystemAdminDelete(SystemAdminDTO systemAdminDTO);
        Task<ApiResponse<dynamic>> SetTenant(TenantDTO tenantDTO);
        Task<TenantModel> GetTenant(Guid tenantId);
    }
}
