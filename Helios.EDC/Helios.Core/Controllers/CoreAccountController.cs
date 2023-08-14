using Helios.Core.Controllers.Base;
using Helios.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Helios.Core.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CoreAccountController : BaseController
    {
        protected readonly IConfiguration configuration;

        public CoreAccountController(ILogger<CoreAccountController> logger, IConfiguration _configuration) : base(logger, _configuration)
        {
            this.configuration = _configuration;
        }

        [HttpPost]
        public IActionResult Login(AccountModel model/*string Email, string Password*/)
        {
            return Ok("success!");
        }
    }
}
