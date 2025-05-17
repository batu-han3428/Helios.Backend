using Helios.Common.DTO;
using Microsoft.AspNetCore.Http;

namespace Helios.Common.Helpers.Api
{
    public static class RequestHelper
    {
        public static BaseDTO GetBaseInformation(this IHeaderDictionary headers)
        {
            var userId = headers["Authorization"];
            var studyId = headers["StudyId"];
            var tenantId = headers["TenantId"];

            return new BaseDTO
            {
                UserId = string.IsNullOrEmpty(userId) ? 0 : Convert.ToInt64(userId),
                StudyId = string.IsNullOrEmpty(studyId) ? 0 : Convert.ToInt64(studyId),
                TenantId = string.IsNullOrEmpty(tenantId) ? 0 : Convert.ToInt64(tenantId)
            };
        }
    }
}
