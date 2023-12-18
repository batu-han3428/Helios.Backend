using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;

namespace Helios.Authentication.Helpers
{
    public interface ITimeZoneHelper
    {
        string? GetTimeZoneInformation (ApplicationUser user);
    }
    public class TimeZoneHelper : ITimeZoneHelper
    {
        private AuthenticationContext _context;
        public TimeZoneHelper(AuthenticationContext context)
        {
            _context = context;
        }

        public string? GetTimeZoneInformation(ApplicationUser user)
        {
            if (user.UserRoles.Count > 0 && (user.UserRoles.First().Role.Name == "TenantAdmin" || user.UserRoles.First().Role.Name == "StudyUser"))
            {
                var data = _context.Tenants.FirstOrDefault(x => x.Id == user.UserRoles.First().TenantId);
                if (data != null) return data.TimeZone;
            }

            return "";
        }
    }
}
