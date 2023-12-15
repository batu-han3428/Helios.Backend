using Helios.Authentication.Entities;
using Helios.Authentication.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace Helios.Authentication.Helpers
{
    public interface ITokenHandler
    {
        Token CreateAccessToken(ApplicationUser user);
        public string CreateRefreshToken();
    }
    public class TokenHandler: ITokenHandler
    {
        public IConfiguration Configuration { get; set; }
        public ITimeZoneHelper TimeZoneHelper { get; set; }
        public TokenHandler(IConfiguration configuration, ITimeZoneHelper timeZoneHelper)
        {
            Configuration = configuration;
            TimeZoneHelper = timeZoneHelper;
        }
        public Token CreateAccessToken(ApplicationUser user)
        {
            List<string> roles = new List<string>();

            foreach (var item in user.UserRoles)
            {
                roles.Add(item.Role?.Name);
            }

            Token tokenInstance = new Token();

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtToken:SecurityKey"]));

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            tokenInstance.Expiration = DateTime.Now.AddDays(1);

            JwtSecurityToken securityToken = new JwtSecurityToken(issuer: Configuration["JwtToken:Issuer"], audience: Configuration["JwtToken:Audience"], expires: tokenInstance.Expiration, notBefore: DateTime.Now, signingCredentials: signingCredentials);


            securityToken.Payload["isAuthenticated"] = true;
            securityToken.Payload["name"] = user.Name;
            securityToken.Payload["roles"] = roles;
            securityToken.Payload["mail"] = user.Email;
            securityToken.Payload["userId"] = user.Id;
            securityToken.Payload["tenantId"] = user.UserRoles.Count > 0 ? user.UserRoles.First().TenantId : "";
            securityToken.Payload["timeZone"] = TimeZoneHelper.GetTimeZoneInformation(user);

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            tokenInstance.AccessToken = tokenHandler.WriteToken(securityToken);

            tokenInstance.AccessToken = tokenInstance.AccessToken;

            tokenInstance.RefreshToken = CreateRefreshToken();

            return tokenInstance;
        }

        public string CreateRefreshToken()
        {
            byte[] number = new byte[32];
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(number);
                return Convert.ToBase64String(number);
            }
        }

    }
}
