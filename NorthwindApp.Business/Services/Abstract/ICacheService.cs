
namespace NorthwindApp.Business.Services.Abstract
{
    /// <summary>
    /// Enhanced cache service interface with better cache management capabilities
    /// </summary>
    public interface ICacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null);
        void Remove(string key);
        void RemoveByPrefix(string prefix);
        void Clear();
        bool Exists(string key);
        IEnumerable<string> GetAllKeys();
    }
}


