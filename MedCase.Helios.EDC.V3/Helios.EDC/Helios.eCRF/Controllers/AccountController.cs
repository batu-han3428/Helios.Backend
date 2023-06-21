using Helios.eCRF.Controllers.Base;
using Helios.eCRF.Models;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Helios.eCRF.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        //public AccountController(ILogger<AccountController> logger, IConfiguration _configuration) : base(logger, _configuration)
        //{
        //}

        [HttpPost]
        public IActionResult Post([FromForm] AccountModel user)
        {
            //using (var client = AuthServiceClient)
            //{
            //    var req = new RestRequest("/api/Core/Login", Method.Get);
            //    req.AddParameter("hshId", u);
            //    var result = await client.ExecuteAsync<string>(req);
            //    var retval = result.Content;

            //    return View("~/Views/Home/Redirect.cshtml", retval);
            //}

            return Ok("Form data received successfully");
        }
    }
}
