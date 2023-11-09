using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.Core.Domains.Entities;
using Helios.eCRF.Models;
using Helios.eCRF.Services;
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
        /// tenant ekler
        /// </summary>
        /// <param name="name">tenant ismi</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<bool> AddTenant(string name)
        {
            var model = new TenantModel { Name = name };
            var result = await _userService.AddTenant(model);
            return result;
            //return Ok("Form data received successfully"); 
        }


        /// <summary>
        /// tenant günceller
        /// </summary>
        /// <param name="model">tenant bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        public async Task<bool> UpdateTenant(TenantModel model)
        {
            var result = await _userService.UpdateTenant(model);
            return result;
            //return Ok("Form data received successfully"); 
        }


        /// <summary>
        /// seçili tenantı getirir
        /// </summary>
        /// <param name="id">tenant id</param>
        /// <returns>tenant bilgisi</returns>
        [HttpGet]
        public async Task<TenantModel> GetTenant(Guid id)
        {
            var result = await _userService.GetTenant(id);

            return result;
        }


        /// <summary>
        /// tenantları listeler
        /// </summary>
        /// <returns>tenant listesi</returns>
        [HttpGet]
        public async Task<List<TenantModel>> GetTenantList()
        {
            var result = await _userService.GetTenantList();

            return result;
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
            if (model.Id == Guid.Empty)
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
        [HttpPost]
        public async Task<IActionResult> PassiveOrActiveUser(UserDTO model)
        {
            var result = await _userService.PassiveOrActiveUser(model);
            return Ok(result);
        }

        #region Permissions


        /// <summary>
        /// çalışmanın rol ve yetkilerini döner
        /// </summary>
        /// <param name="studyId">çalışma id</param>
        /// <returns>rol ve yetkiler</returns>
        [HttpGet("{studyId}")]
        public async Task<IActionResult> GetPermissionRoleList(Guid studyId)
        {
            var result = await _userService.GetPermissionRoleList(studyId);
            return Ok(result);
        }


        /// <summary>
        /// çalışmanın rollerini getirir
        /// </summary>
        /// <param name="studyId">çalışma id</param>
        /// <returns>roller</returns>
        [HttpGet("{studyId}")]
        public async Task<IActionResult> GetRoleList(Guid studyId)
        {
            var result = await _userService.GetRoleList(studyId);
            return Ok(result);
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
        public async Task<IActionResult> GetStudyUserList(Guid studyId)
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
        public async Task<IActionResult> GetStudyUser(string email, Guid studyId)
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
        public async Task<IActionResult> GetTenantUserList(Guid tenantId)
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

        #region SSO


        /// <summary>
        /// kullanıcının bulunduğu tenantları listeler
        /// </summary>
        /// <param name="userId">kullanıcı id</param>
        /// <returns>tenant listesi</returns>
        [HttpGet("{userId}")]
        [Authorize(Roles = "StudyUser")]
        public async Task<List<TenantUserModel>> GetUserTenantList(Guid userId)
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
        public async Task<List<SSOUserStudyModel>> GetUserStudiesList(Guid tenantId, Guid userId)
        {
            return await _userService.GetUserStudiesList(tenantId, userId);
        }
        #endregion
    }
}
