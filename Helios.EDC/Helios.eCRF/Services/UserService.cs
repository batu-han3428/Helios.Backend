using Helios.eCRF.Models;
using Helios.eCRF.Services.Base;
using Helios.eCRF.Services.Interfaces;
using Newtonsoft.Json;
using RestSharp;

namespace Helios.eCRF.Services
{
    public class UserService : ApiBaseService, IUserService
    {
        public UserService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> AddTenant(TenantModel model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/AddTenant", Method.Post);
                //req.AddHeader("Name", name);
                //req.AddParameter("Name", name);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
                return result.IsSuccessful;
            }
        }

        public async Task<bool> UpdateTenant(TenantModel model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/UpdateTenant", Method.Post);
                //req.AddHeader("Name", name);
                //req.AddParameter("Name", name);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
                return result.IsSuccessful;
            }
        }

        public async Task<TenantModel> GetTenant(Guid id)
        {
            var tenant = new TenantModel();

            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/GetTenant", Method.Get);
                var result = await client.ExecuteAsync(req);
                tenant = JsonConvert.DeserializeObject<TenantModel>(result.Content);
            }

            return tenant;
        }

        public async Task<UserDTO> GetUserByEmail(string mail)
        {
            var user = new UserDTO();

            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/GetUserByEmail", Method.Get);
                req.AddParameter("mail", mail);
                var result = await client.ExecuteAsync(req);
                user = JsonConvert.DeserializeObject<UserDTO>(result.Content);
            }

            return user;
        }

        public async Task<dynamic> AddUser(UserDTO model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/AddUser", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync<dynamic>(req);
                return result.Data;
            }
        }

        public async Task<bool> PassiveOrActiveUser(UserDTO model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/PassiveOrActiveUser", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
            }

            return false;
        }

        public async Task<bool> UpdateUser(UserDTO model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/UpdateUser", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
            }

            return false;
        }

        public async Task<List<TenantModel>> GetTenantList()
        {
            var tenantList = new List<TenantModel>();

            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AdminUser/GetTenantList", Method.Get);
                var result = await client.ExecuteAsync(req);
                tenantList = JsonConvert.DeserializeObject<List<TenantModel>>(result.Content);
            }

            return tenantList;
        }
    }
}
