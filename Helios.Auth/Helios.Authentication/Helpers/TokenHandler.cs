using Helios.Authentication.Domains.Entities;
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
        Token UpdateJwtToken(JwtDTO jwtToken);
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
            securityToken.Payload["lastName"] = user.LastName;
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

        public Token UpdateJwtToken(JwtDTO jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var oldToken = tokenHandler.ReadToken(jwtToken.Token) as JwtSecurityToken;

            if (oldToken == null)
            {
                throw new SecurityTokenException("Invalid JWT format");
            }

            List<string> roles = oldToken?.Claims
                .Where(c => c.Type == "roles")
                .Select(c => c.Value)
                .ToList();
            var oldName = oldToken?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var oldLastName = oldToken?.Claims.FirstOrDefault(c => c.Type == "lastName")?.Value;
            var oldMail = oldToken?.Claims.FirstOrDefault(c => c.Type == "mail")?.Value;
            var oldUserId = Convert.ToInt64(oldToken?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
            var oldTenantId = Convert.ToInt64(oldToken?.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value);
            var studyId = oldToken?.Claims.FirstOrDefault(c => c.Type == "studyId")?.Value;
            var oldStudyId = studyId != null && studyId != "" ? Convert.ToInt64(studyId) : 0;
            var oldTimeZone = oldToken?.Claims.FirstOrDefault(c => c.Type == "timeZone")?.Value;

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
            securityToken.Payload["name"] = jwtToken.Name != null && oldName != jwtToken.Name ? jwtToken.Name : oldName;
            securityToken.Payload["lastName"] = jwtToken.LastName != null && oldLastName != jwtToken.LastName ? jwtToken.LastName : oldLastName;
            securityToken.Payload["roles"] = jwtToken?.Roles != null ? roles.OrderBy(x => x).SequenceEqual(jwtToken?.Roles?.OrderBy(x => x)) ? roles : jwtToken.Roles : roles;
            securityToken.Payload["mail"] = jwtToken.Mail != null && oldMail != jwtToken.Mail ? jwtToken.Mail : oldMail;
            securityToken.Payload["userId"] = jwtToken.UserId != null && oldUserId != jwtToken.UserId ? jwtToken.UserId : oldUserId;
            securityToken.Payload["tenantId"] = jwtToken.TenantId != null && oldTenantId != jwtToken.TenantId ? jwtToken.TenantId == 0 ? "" : jwtToken.TenantId : oldTenantId;
            securityToken.Payload["studyId"] = jwtToken.StudyId != null && oldStudyId != jwtToken.StudyId ? jwtToken.StudyId == 0 ? "" : jwtToken.StudyId : oldStudyId;
            securityToken.Payload["timeZone"] = jwtToken.TimeZone != null && oldTimeZone != jwtToken.TimeZone ? jwtToken.TimeZone : oldTimeZone;

            JwtSecurityTokenHandler newTokenHandler = new JwtSecurityTokenHandler();

            tokenInstance.AccessToken = newTokenHandler.WriteToken(securityToken);

            tokenInstance.RefreshToken = CreateRefreshToken();

            return tokenInstance;
        }
    }
}
