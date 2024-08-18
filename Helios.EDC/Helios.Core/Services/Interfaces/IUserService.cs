using Helios.Common.DTO;
using RestSharp;

namespace Helios.Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<RestResponse<List<AspNetUserDTO>>> GetUserList(List<Int64> AuthUserIds);
    }
}
