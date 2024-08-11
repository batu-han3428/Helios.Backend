namespace Helios.Shared.Services.Interfaces
{
    public interface ICacheService
    {
        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
        public T GetData<T>(string key);
        public object RemoveData(string key);
    }
}
