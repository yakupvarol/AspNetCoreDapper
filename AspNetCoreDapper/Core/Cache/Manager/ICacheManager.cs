using System;
using System.Collections.Generic;

namespace Core.Cache.Manager
{
    public interface ICacheManager
    {
        void Set<T>(string key, T value);
        void Set<T>(string key, T value, int duration);
        T Get<T>(string key) where T : class;
        bool Contains(string key);
        Int64 Count<T>(string key);
        void Remove(string key);
        void RemoveRange(string key);
        void Reset();
        IList<T> FetchDataIList<T>(string key, string countkey, int duration, IEnumerable<T> rs, int skip, int take);

        IEnumerable<T> FetchDataIEnumerable<T>(string key, string countkey, int duration, IEnumerable<T> rs, int skip, int take);
        T FetchData<T>(string key, int duration, IEnumerable<T> rs) where T : class;
    }
}
