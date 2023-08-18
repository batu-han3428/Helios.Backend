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

        public async Task<bool> AddTenant(TenantModel model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AuthAccount/AddTenant", Method.Post);
                //req.AddHeader("Name", name);
                //req.AddParameter("Name", name);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
            }
            //return Task.FromResult(result.);
            return false;
        }

        public async Task<List<TenantModel>> GetTenantList()
        {
            var tenantList = new List<TenantModel>();

            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AuthAccount/GetTenantList", Method.Get);
                var result = await client.ExecuteAsync(req);
                tenantList = JsonConvert.DeserializeObject<List<TenantModel>>(result.Content);
            }

            return tenantList;
        }

    }
}
