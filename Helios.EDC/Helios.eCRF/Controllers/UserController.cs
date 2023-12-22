using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.eCRF.Models;
using Helios.eCRF.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helios.eCRF.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        /// <summary>
        /// kullanıcının mail adresine göre bilgilerini getirir
        /// </summary>
        /// <param name="mail">kullanıcının mail adresi</param>
        /// <returns>kullanıcının bilgileri</returns>
        [HttpGet]
        public async Task<UserDTO> GetUserByEmail(string mail)
        {
            var result = await _userService.GetUserByEmail(mail);
            return result;
        }


        /// <summary>
        /// admin kayıt eder
        /// </summary>
        /// <param name="model">kayıt edilecek kullanıcının bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> SaveAdminUser(UserDTO model)
        {
            if (model.Id == 0)
            {
                var result = await _userService.AddUser(model);
                return Ok(result);
            }
            else
            {
                //var result = await _userService.UpdateUser(model);
                return Ok(/*result*/);
            }
        }


        /// <summary>
        /// seçili kullanıcının durumunu aktif yada pasif olarak ayarlar
        /// </summary>
        /// <param name="model">kullanıcı bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        /// <param name="id">tenant id</param>
        /// <returns>tenant bilgisi</returns>
        [HttpPost]
        public async Task<IActionResult> PassiveOrActiveUser(UserDTO model)
        {
            var result = await _userService.PassiveOrActiveUser(model);
            return Ok(result);
        }

        #region Tenants

        /// <summary>
        /// tenantları çalışma sayılarıyla listeler
        /// </summary>
        /// <returns>tenant listesi</returns>
        [HttpGet]
        public async Task<IActionResult> GetTenantList()
        {
            try
            {
                var result = await _userService.GetTenantList();
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// tenantları listeler
        /// </summary>
        /// <returns>tenant listesi</returns>
        [HttpGet]
        public async Task<IActionResult> GetAuthTenantList()
        {
            try
            {
                var result = await _userService.GetAuthTenantList();
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// seçili tenant bilgilerini getirir
        /// </summary>
        /// <param name="tenantId">tenant id</param>
        /// <returns>tenant bilgileri</returns>
        [HttpGet("{tenantId}")]
        public async Task<IActionResult> GetTenant(Int64 tenantId)
        {
            try
            {
                var result = await _userService.GetTenant(tenantId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// tenant siler ya da günceller
        /// </summary>
        /// <param name="model">tenant bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> SetTenant([FromForm] TenantDTO tenantDTO)
        {
            var result = await _userService.SetTenant(tenantDTO);
            return Ok(result);
        }
        #endregion

        #region Permissions

        /// <summary>
        /// çalışmanın rol ve yetkilerini getirir
        /// </summary>
        /// <param name="studyId">çalışma id</param>
        /// <returns>rol ve yetkiler</returns>
        [HttpGet("{studyId}")]
        public async Task<IActionResult> GetPermissionRoleList(Int64 studyId)
        {
            try
            {
                var result = await _userService.GetPermissionRoleList(studyId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }


        /// <summary>
        /// çalışmanın rollerini getirir
        /// </summary>
        /// <param name="studyId">çalışma id</param>
        /// <returns>roller</returns>
        [HttpGet("{studyId}")]
        public async Task<IActionResult> GetRoleList(Int64 studyId)
        {
            var result = await _userService.GetRoleList(studyId);
            return Ok(result);
        }


        /// <summary>
        /// rolün kullanıcılarını getirir
        /// </summary>
        /// <param name="roleId">rol id</param>
        /// <returns>rolün kullanıcıları</returns>
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleUsers(Int64 roleId)
        {
            var result = await _userService.GetRoleUsers(roleId);
            return Ok(result);
        }


        /// <summary>
        /// çalışmadaki kullanıcıları ve rollerini listeler
        /// </summary>
        /// <param name="studyId">çalışma id</param>
        /// <returns>çalışmada ki kullanıcılar ve rolleri</returns>
        [HttpGet("{studyId}")]
        public async Task<IActionResult> GetStudyRoleUsers(Int64 studyId)
        {
            try
            {
                var result = await _userService.GetStudyRoleUsers(studyId);

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }


        /// <summary>
        /// yetki günceller
        /// </summary>
        /// <param name="setPermissionModel">yetki bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> SetPermission(SetPermissionModel setPermissionModel)
        {
            var result = await _userService.SetPermission(setPermissionModel);
            return Ok(result);
        }


        /// <summary>
        /// rol ekler ya da günceller
        /// </summary>
        /// <param name="userPermission">rol bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> AddOrUpdatePermissionRol(UserPermissionModel userPermission)
        {
            var result = await _userService.AddOrUpdatePermissionRol(userPermission);
            return Ok(result);
        }


        /// <summary>
        /// rol siler
        /// </summary>
        /// <param name="userPermission">rol bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteRole(UserPermissionModel userPermission)
        {
            var result = await _userService.DeleteRole(userPermission);
            return Ok(result);
        }
        #endregion

        #region Study User

        /// <summary>
        /// çalışmanın kullanıcılarını listeler
        /// </summary>
        /// <param name="studyId">çalışma id</param>
        /// <returns>kullanıcılar</returns>
        [HttpGet("{studyId}")]
        public async Task<IActionResult> GetStudyUserList(Int64 studyId)
        {
            var result = await _userService.GetStudyUserList(studyId);
            return Ok(result);
        }


        /// <summary>
        /// mail adresi girilen kullanıcının bilgilerini getirir. kullanıcı yoksa boş döner. kullanıcı çalışmada kayıtlı ise uyarı döner
        /// </summary>
        /// <param name="email">kullanıcı email</param>
        /// <param name="studyId">çalışma id</param>
        /// <returns>kullanıcı bilgisi ya da uyarı</returns>
        [HttpGet("{email}/{studyId}")]
        public async Task<IActionResult> GetStudyUser(string email, Int64 studyId)
        {
            var result = await _userService.GetStudyUser(email, studyId);
            return Ok(result);
        }


        /// <summary>
        /// çalışmaya kullanıcı ekler veya günceller
        /// </summary>
        /// <param name="studyUserModel">kullanıcı bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> SetStudyUser(StudyUserModel studyUserModel)
        {
            var result = await _userService.SetStudyUser(studyUserModel);
            return Ok(result);
        }


        /// <summary>
        /// seçili kullanıcıyı aktif veya pasif hale getirir
        /// </summary>
        /// <param name="studyUserModel">kullanıcı bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> ActivePassiveStudyUser(StudyUserModel studyUserModel)
        {
            var result = await _userService.ActivePassiveStudyUser(studyUserModel);
            return Ok(result);
        }


        /// <summary>
        /// seçili kullanıcıları aktif veya pasif hale getirir
        /// </summary>
        /// <param name="studyUserModel">kullanıcı bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> ActivePassiveStudyUsers(StudyUserModel studyUserModel)
        {
            var result = await _userService.ActivePassiveStudyUsers(studyUserModel);
            return Ok(result);
        }

        /// <summary>
        /// Seçili kullanıcıyı siler
        /// </summary>
        /// <param name="studyUserModel">kullanıcı bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteStudyUser(StudyUserModel studyUserModel)
        {
            var result = await _userService.DeleteStudyUser(studyUserModel);
            return Ok(result);
        }


        /// <summary>
        /// Seçili kullanıcının şifresini sıfırlar ve mail gönderir
        /// </summary>
        /// <param name="studyUserModel">kullanıcı bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> UserResetPassword(StudyUserModel studyUserModel)
        {
            var result = await _userService.UserResetPassword(studyUserModel);
            return Ok(result);
        }
        #endregion

        #region Tenant User

        /// <summary>
        /// tenantın kullanıcılarını listeler
        /// </summary>
        /// <param name="tenantId">tenant id</param>
        /// <returns>kullanıcılar</returns>
        [HttpGet("{tenantId}")]
        public async Task<IActionResult> GetTenantUserList(Int64 tenantId)
        {
            var result = await _userService.GetTenantUserList(tenantId);
            return Ok(result);
        }

        /// <summary>
        /// tenant kullanıcısını günceller
        /// </summary>
        /// <param name="studyUserModel">kullanıcı bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> SetTenantUser(TenantUserModel tenantUserModel)
        {
            var result = await _userService.SetTenantUser(tenantUserModel);
            return Ok(result);
        }
        #endregion

        #region System Admin User

        /// <summary>
        /// sistem admin ekler veya günceller
        /// </summary>
        /// <param name="systemAdminDTO">sistem admin bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> SetSystemAdminUser(SystemAdminDTO systemAdminDTO)
        {
            var result = await _userService.SetSystemAdminUser(systemAdminDTO);
            return Ok(result);
        }


        /// <summary>
        /// sistem admin kullanıcılarını listeler
        /// </summary>
        /// <returns>kullanıcılar</returns>
        [HttpGet]
        public async Task<IActionResult> GetSystemAdminUserList()
        {
            try
            {
                var result = await _userService.GetSystemAdminUserList();
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }


        /// <summary>
        /// seçili sistem admin kullanıcısını aktif veya pasif hale getirir
        /// </summary>
        /// <param name="systemAdminDTO">kullanıcı bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> SystemAdminActivePassive(SystemAdminDTO systemAdminDTO)
        {
            var result = await _userService.SystemAdminActivePassive(systemAdminDTO);
            return Ok(result);
        }


        /// <summary>
        /// seçili sistem admin kullanıcısının şifresini sıfırlar
        /// </summary>
        /// <param name="systemAdminDTO">kullanıcı bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> SystemAdminResetPassword(SystemAdminDTO systemAdminDTO)
        {
            var result = await _userService.SystemAdminResetPassword(systemAdminDTO);
            return Ok(result);
        }


        /// <summary>
        /// seçili sistem admin kullanıcısını siler
        /// </summary>
        /// <param name="systemAdminDTO">kullanıcı bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> SystemAdminDelete(SystemAdminDTO systemAdminDTO)
        {
            var result = await _userService.SystemAdminDelete(systemAdminDTO);
            return Ok(result);
        }
        #endregion

        #region Tenant Admin User

        /// <summary>
        /// rol seçimine göre kullanıcı ekler veya günceller
        /// </summary>
        /// <param name="systemAdminDTO">kullanıcı bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> SetSystemAdminAndTenantAdminUser(SystemAdminDTO systemAdminDTO)
        {
            var result = await _userService.SetSystemAdminAndTenantAdminUser(systemAdminDTO);
            return Ok(result);
        }

        /// <summary>
        /// tenant admin ve sistem admin kullanıcılarını listeler
        /// </summary>
        /// <returns>kullanıcılar</returns>
        [HttpGet]
        public async Task<IActionResult> GetTenantAndSystemAdminUserList()
        {
            try
            {
                var result = await _userService.GetTenantAndSystemAdminUserList();
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }


        /// <summary>
        /// seçili tenant admin ve system adminleri siler
        /// </summary>
        /// <param name="tenantAndSystemAdminDTO">kullanıcı bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<IActionResult> TenantAndSystemAdminDelete(TenantAndSystemAdminDTO tenantAndSystemAdminDTO)
        {
            var result = await _userService.TenantAndSystemAdminDelete(tenantAndSystemAdminDTO);
            return Ok(result);
        }
        
        #endregion

        #region SSO


        /// <summary>
        /// kullanıcının bulunduğu tenantları listeler
        /// </summary>
        /// <param name="userId">kullanıcı id</param>
        /// <returns>tenant listesi</returns>
        [HttpGet("{userId}")]
        [Authorize(Roles = "StudyUser")]
        public async Task<List<TenantUserModel>> GetUserTenantList(Int64 userId)
        {
            return await _userService.GetUserTenantList(userId);
        }


        /// <summary>
        /// kullanıcının bulunduğu tenantları listeler
        /// </summary>
        /// <param name="userId">kullanıcı id</param>
        /// <returns>tenant listesi</returns>
        [HttpGet("{tenantId}/{userId}")]
        [Authorize(Roles = "StudyUser")]
        public async Task<List<SSOUserStudyModel>> GetUserStudiesList(Int64 tenantId, Int64 userId)
        {
            return await _userService.GetUserStudiesList(tenantId, userId);
        }
        #endregion
    }
}
