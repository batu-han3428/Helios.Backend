using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Serializers;

namespace Helios.eCRF.Controllers.Base
{
    public class BaseController : Controller
    {
        protected readonly IConfiguration configuration;

        protected readonly ILogger<AccountController> _logger;
        protected RestClient AuthServiceClient { get { return new RestClient(new Uri(configuration["Halios.Auth:Authentication"])); } }
        public BaseController(ILogger<AccountController> logger, IConfiguration _configuration, IDeserializer serializer, string baseUrl)
        {
            _logger = logger;
            configuration = _configuration;
        }
    }
}
