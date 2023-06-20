using Helios.Core.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Helios.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        protected readonly IConfiguration configuration;

        public AccountController(ILogger<AccountController> logger, IConfiguration _configuration) : base(logger, _configuration)
        {
            this.configuration = _configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Ok("success!");
        }
    }
}
