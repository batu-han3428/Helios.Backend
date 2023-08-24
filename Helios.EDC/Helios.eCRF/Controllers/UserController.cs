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

        [HttpPost]
        public async Task<bool> AddTenant(string name)
        {
            var model = new TenantModel { Name = name };
            var result = await _userService.AddTenant(model);
            return result;
            //return Ok("Form data received successfully"); 
        }
        
        [HttpPost]
        public async Task<bool> UpdateTenant(TenantModel model)
        {
            var result = await _userService.AddTenant(model);
            return result;
            //return Ok("Form data received successfully"); 
        }
        
        [HttpGet]
        public async Task<TenantModel> GetTenant(Guid id)
        {
            var result = await _userService.GetTenant(id);

            return result;
        }

        [HttpGet]
        public async Task<List<TenantModel>> GetTenantList()
        {
            var result = await _userService.GetTenantList();

            return result;
        }

        [HttpGet]
        public async Task<UserDTO> GetUserByEmail(string mail)
        {
            var result = await _userService.GetUserByEmail(mail);
            return result;
        }

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

        [HttpPost]
        public async Task<IActionResult> PassiveOrActiveUser(UserDTO model)
        {
            var result = await _userService.PassiveOrActiveUser(model);
            return Ok(result);
        }
    }
}
