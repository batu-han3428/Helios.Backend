﻿using Helios.eCRF.Models;
using Helios.eCRF.Services.Base;
using Helios.eCRF.Services.Interfaces;
using RestSharp;

namespace Helios.eCRF.Services
{
    public class AuthService : ApiBaseService, IAuthService
    {
        public AuthService(IConfiguration configuration, string baseUrl = "services:auth") : base(configuration, baseUrl)
        {
        }

        public async Task<bool> LoginAsync(AccountModel model)
        {
            var req = new RestSharp.RestRequest("/Account/Login", RestSharp.Method.Post);
            req.AddBody(new { model.Email, model.Password});
            var result = await base.RestClient.ExecuteAsync(req, default);
            //return Task.FromResult(result.);
            return false;
        }
    }
}
