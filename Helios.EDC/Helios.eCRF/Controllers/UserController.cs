using Helios.eCRF.Models;
using Helios.eCRF.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Helios.eCRF.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public async Task<bool> AddTenant(string name)
        {
            var model = new TenantModel { Name = name };
            var result = await userService.AddTenant(model);
            return result;
            //return Ok("Form data received successfully"); 
        }
        
        [HttpPost]
        public async Task<bool> UpdateTenant(TenantModel model)
        {
            var result = await userService.AddTenant(model);
            return result;
            //return Ok("Form data received successfully"); 
        }
        
        [HttpGet]
        public async Task<TenantModel> GetTenant(Guid id)
        {
            var result = await userService.GetTenant(id);

            return result;
        }

        [HttpGet]
        public async Task<List<TenantModel>> GetTenantList()
        {
            var result = await userService.GetTenantList();

            return result;
        }

        [HttpGet]
        public async Task<UserDTO> GetUserByEmail(string mail)
        {
            var result = await userService.GetUserByEmail(mail);
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> SaveAdminUser(UserDTO model)
        {
            if (model.Id == Guid.Empty)
            {
                var result = await userService.AddUser(model);
                return Ok(result);
            }
            else
            {
                var result = await userService.UpdateUser(model);
                return Ok(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PassiveOrActiveUser(UserDTO model)
        {
            var result = await userService.PassiveOrActiveUser(model);
            return Ok(result);
        }
    }
}
