namespace Helios.Authentication.Services.Interfaces
{
    public interface ICoreService
    {
        Task<List<Int64>> GetUserStudyIds(Int64 userId);
    }
}
