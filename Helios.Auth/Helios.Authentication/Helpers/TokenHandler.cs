using Helios.Authentication.Entities;
using Helios.Authentication.Models;
using Helios.Common.DTO;
using Helios.Common.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace Helios.Authentication.Helpers
{
    public interface ITokenHandler
    {
        Token CreateAccessToken(ApplicationUser user, List<Int64> tenantIds = null, List<Int64> studyIds = null);
        Token CreateAccessTokenFromOldJwt(SSOLoginDTO sSOLoginDTO);
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
        public Token CreateAccessToken(ApplicationUser user, List<Int64> tenantIds = null, List<Int64> studyIds = null)
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
            securityToken.Payload["tenantId"] = tenantIds != null ? tenantIds : "";
            securityToken.Payload["studyId"] = studyIds != null ? studyIds : "";
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

        public Token CreateAccessTokenFromOldJwt(SSOLoginDTO sSOLoginDTO)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var oldToken = tokenHandler.ReadToken(sSOLoginDTO.Jwt) as JwtSecurityToken;

            if (oldToken == null)
            {
                throw new SecurityTokenException("Invalid JWT format");
            }

            List<string> roles = oldToken?.Claims
                .Where(c => c.Type == "roles")
                .Select(c => c.Value)
                .ToList();
            roles.RemoveAll(x => (sSOLoginDTO.StudyId != null && x != Roles.StudyUser.ToString()) || (sSOLoginDTO.StudyId == null && x != Roles.TenantAdmin.ToString()));

            Token tokenInstance = new Token();

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtToken:SecurityKey"]));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            tokenInstance.Expiration = DateTime.Now.AddDays(1);

            JwtSecurityToken securityToken = new JwtSecurityToken(
                issuer: Configuration["JwtToken:Issuer"],
                audience: Configuration["JwtToken:Audience"],
                expires: tokenInstance.Expiration,
                notBefore: DateTime.Now,
                signingCredentials: signingCredentials
            );

            securityToken.Payload["isAuthenticated"] = true;
            securityToken.Payload["name"] = oldToken?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            securityToken.Payload["roles"] = roles;
            securityToken.Payload["mail"] = oldToken?.Claims.FirstOrDefault(c => c.Type == "mail")?.Value;
            securityToken.Payload["userId"] = Convert.ToInt64(oldToken?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
            securityToken.Payload["tenantId"] = sSOLoginDTO.TenantId != 0 ? sSOLoginDTO.TenantId : "";
            securityToken.Payload["studyId"] = sSOLoginDTO.StudyId != 0 ? sSOLoginDTO.StudyId : "";
            securityToken.Payload["timeZone"] = oldToken?.Claims.FirstOrDefault(c => c.Type == "timeZone")?.Value;

            JwtSecurityTokenHandler newTokenHandler = new JwtSecurityTokenHandler();

            tokenInstance.AccessToken = newTokenHandler.WriteToken(securityToken);

            tokenInstance.RefreshToken = CreateRefreshToken();

            return tokenInstance;
        }
    }
}
