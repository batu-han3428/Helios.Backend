using Helios.Common.DTO;
using Helios.eCRF.Models;
using Helios.eCRF.Services.Base;
using Helios.eCRF.Services.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Mail;

namespace Helios.eCRF.Services
{
    public class AuthService : ApiBaseService, IAuthService
    {
        public AuthService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<ApiResponse<dynamic>> LoginAsync(AccountModel model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AuthAccount/Login", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
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

        public async Task ContactUsMailAsync(ContactUsDTO contactUsDTO)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AuthAccount/ContactUsMail", Method.Post);
                req.AddJsonBody(contactUsDTO);
                var result = await client.ExecuteAsync(req);
            }
        }

        public async Task<dynamic> SaveForgotPassword(string Mail)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest($"AuthAccount/SaveForgotPassword?Mail={Mail}", Method.Post);
                var result = await client.ExecuteAsync<dynamic>(req);
                return result.Data;
            }
        }

        public async Task<dynamic> ResetPasswordGet(string code, string username, bool firstPassword)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AuthAccount/ResetPasswordGet", Method.Get);
                req.AddParameter("code", code);
                req.AddParameter("username", username);
                req.AddParameter("firstPassword", firstPassword);
                var result = await client.ExecuteAsync<dynamic>(req);
                return result.Data;
            }
        }

        public async Task<dynamic> ResetPasswordPost(ResetPasswordViewModel model)
        {
            using (var client = AuthServiceClient)
            {
                var req = new RestRequest("AuthAccount/ResetPasswordPost", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync<dynamic>(req);
                return result.Data;
            }
        }
    }
}
