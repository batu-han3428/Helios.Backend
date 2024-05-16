using Helios.Common.DTO;
using Helios.Common.Model;

namespace Helios.Core.Services.Interfaces
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
        object RemoveData(string key);
        Task<ApiResponse<dynamic>> SetSubjectDetailMenu(Int64 studyId);
        Task<List<SubjectDetailMenuModel>> GetSubjectDetailMenu(Int64 studyId);
    }
}
