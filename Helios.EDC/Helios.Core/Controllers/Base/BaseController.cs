using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Helios.Core.Controllers.Base
{
    public class BaseController : Controller
    {
        protected readonly IConfiguration configuration;

        protected readonly ILogger<CoreAccountController> _logger;
        protected RestClient AuthServiceClient { get { return new RestClient(new Uri(configuration["Halios.Auth:Authentication"])); } }
        public BaseController(ILogger<CoreAccountController> logger, IConfiguration _configuration)
        {
            _logger = logger;
            configuration = _configuration;
        }
    }
}
