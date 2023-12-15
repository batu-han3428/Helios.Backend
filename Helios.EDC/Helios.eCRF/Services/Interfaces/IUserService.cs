using Helios.eCRF.Models;
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
        Task<List<UserPermissionDTO>> GetPermissionRoleList(Int64 studyId);
        Task<ApiResponse<dynamic>> SetPermission(SetPermissionModel setPermissionModel);
        Task<List<StudyUserDTO>> GetStudyUserList(Int64 studyId);
        Task<ApiResponse<dynamic>> SetStudyUser(StudyUserModel studyUserModel);
        Task<ApiResponse<dynamic>> ActivePassiveStudyUser(StudyUserModel studyUserModel);
        Task<ApiResponse<dynamic>> ActivePassiveStudyUsers(StudyUserModel studyUserModel);
        Task<ApiResponse<dynamic>> DeleteStudyUser(StudyUserModel studyUserModel);
        Task<ApiResponse<dynamic>> UserResetPassword(StudyUserModel model);
        Task<ApiResponse<StudyUserDTO>> GetStudyUser(string email, Int64 studyId);
        Task<List<UserPermissionDTO>> GetRoleList(Int64 studyId);
        Task<List<TenantUserDTO>> GetTenantUserList(Int64 tenantId);
        Task<ApiResponse<dynamic>> SetTenantUser(TenantUserModel studyUserModel);
        Task<List<TenantUserModel>> GetUserTenantList(Int64 userId);
        Task<List<SSOUserStudyModel>> GetUserStudiesList(Int64 tenantId, Int64 userId);
        Task<List<UserPermissionRoleModel>> GetRoleUsers(Int64 roleId);
        Task<List<UserPermissionRoleModel>> GetStudyRoleUsers(Int64 studyId);
        Task<ApiResponse<dynamic>> SetSystemAdminUser(SystemAdminDTO systemAdminDTO);
        Task<List<SystemUserModel>> GetSystemAdminUserList();
        Task<ApiResponse<dynamic>> SystemAdminActivePassive(SystemAdminDTO systemAdminDTO);
        Task<ApiResponse<dynamic>> SystemAdminResetPassword(SystemAdminDTO systemAdminDTO);
        Task<ApiResponse<dynamic>> SystemAdminDelete(SystemAdminDTO systemAdminDTO);
        Task<ApiResponse<dynamic>> SetTenant(TenantDTO tenantDTO);
        Task<TenantModel> GetTenant(Guid tenantId);
    }
}
