using Helios.Common.DTO;
using Helios.Common.Model;
using RestSharp;

namespace Helios.Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<dynamic>> RemoveUserPermissions(Int64 studyId);
    }
}
