namespace Libx
{
    //public interface MCacheBase
    //{
    //    T Get<T>(string cacheKey);
    //    bool TryGet<T>(string cacheKey, out T value);
    //    void Set<T>(string cacheKey, T value);
    //    //void Set<T>(string cacheKey, T value, int duration);
    //    void Clear(string cacheKey);
    //    IEnumerable<KeyValuePair<string, object>> GetAll();
    //    T Get<T>(string cacheKey, Func<T> getItemCallback, int minutes, int second = 0) where T : class;
    //    T Get<T>(string cacheKey, Func<T> getItemCallback) where T : class;
    //}
    //===================================================================================================
    public abstract class MCacheBase //: IMCache
    {
        public int CacheDuration
        {
            get;
            set;
        }

        //private readonly int defaultCacheDurationInMinutes = 30;

        //public MCacheBase()
        //{
        //    CacheDuration = defaultCacheDurationInMinutes;
        //    _cache = InitCache();
        //}
        //public MCacheBase(int durationInMinutes)
        //{
        //    CacheDuration = durationInMinutes;
        //    _cache = InitCache();
        //}

        public virtual T Get<T>(string cacheKey) where T : class
        {
            throw new NotImplementedException();
        }
        public virtual T Get<T>(string cacheKey, Func<T> getItemCallback) where T : class
        {
            throw new NotImplementedException();
        }

        public virtual T Get<T>(string cacheKey, Func<T> getItemCallback, int minutes, int second = 0) where T : class
        {
            throw new NotImplementedException();
        }

        public virtual bool TryGet<T>(string cacheKey, out T value)
        {
            throw new NotImplementedException();
        }

        public virtual void Set<T>(string cacheKey, T value)
        {
            throw new NotImplementedException();
        }

        public virtual void Set<T>(string cacheKey, T value, int minutes, int second = 0) where T : class
        {
            throw new NotImplementedException();
        }


        public virtual void Clear(string cacheKey)
        {
            throw new NotImplementedException();
        }
        public virtual void Remove(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public virtual void ClearAll()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
