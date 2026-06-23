namespace TimorINSSBackEnd.Cache
{
    public interface ICacheProvider
    {
        T GetFromCache<T>(string key) where T : class;

        void SetCache<T>(string key, T value, long size) where T : class;

        void ClearCache(string key);

        public void Reset();
    }
}