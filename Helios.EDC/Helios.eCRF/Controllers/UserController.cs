using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.eCRF.Models;
using Helios.eCRF.Services.Interfaces;
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
                var result = await _userService.UpdateUser(model);
                return Ok(result);
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
    }
}
