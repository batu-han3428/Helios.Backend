using Helios.eCRF.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Helios.eCRF.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(ILogger<AccountController> logger, IConfiguration _configuration) : base(logger, _configuration)
        {
        }

        public async Task<IActionResult> Login(string u)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("/api/Core/Login", Method.Get);
                req.AddParameter("hshId", u);
                var result = await client.ExecuteAsync<string>(req);
                var retval = result.Content;

                return View("~/Views/Home/Redirect.cshtml", retval);
            }
        }
    }
}
