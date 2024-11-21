namespace JobCandidateHub.Services
{
    public interface ICacheService
    {
        T? GetFromCache<T>(string key);
        void SetInCache<T>(string key, T item, TimeSpan? absoluteExpirationRelativeToNow = null);
        void RemoveFromCache(string key);
    }
}
