using Helios.eCRF.Controllers.Base;
using Helios.eCRF.Models;
using Helios.eCRF.Services;
using Helios.eCRF.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Serializers;

namespace Helios.eCRF.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService authService;
        
        public AccountController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] AccountModel user)
        {
            var a = authService.LoginAsync(user);
            //using (var client = AuthServiceClient)
            //{
            //    var req = new RestRequest("/api/Account/Login", Method.Get);
            //    req.AddJsonBody(user);
            //    //req.AddParameter("hshId", u);
            //    var result = await client.ExecuteAsync<string>(req);
            //    var retval = result.Content;

            //    //return View("~/Views/Home/Redirect.cshtml", retval);
            //}

            return Ok("Form data received successfully");
        }
    }
}
