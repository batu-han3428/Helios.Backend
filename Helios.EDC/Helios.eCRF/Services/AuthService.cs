using Helios.eCRF.Models;
using Helios.eCRF.Services.Base;
using Helios.eCRF.Services.Interfaces;
using Newtonsoft.Json;
using RestSharp;

namespace Helios.eCRF.Services
{
    public class AuthService : ApiBaseService, IAuthService
    {
        public AuthService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> LoginAsync(AccountModel model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AuthAccount/Login", Method.Post);
                //var req = new RestRequest("CoreAccount/Login", Method.Post);
                req.AddJsonBody(model);
                //req.AddParameter("Email", model.Email);
                //req.AddParameter("Parameter", model.Password);
                var result = await client.ExecuteAsync(req);
            }
            //return Task.FromResult(result.);
            return false;
        }

        public async Task<bool> SendNewPasswordForUser(Guid userId)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AuthAccount/SendNewPasswordForUser", Method.Get);
                req.AddParameter("userId", userId);
                var result = await client.ExecuteAsync(req);
            }

            return false;
        }

        public async Task<bool> UserProfileResetPassword(UserDTO model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AuthAccount/UserProfileResetPassword", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
            }

            return false;
        }
    }
}
