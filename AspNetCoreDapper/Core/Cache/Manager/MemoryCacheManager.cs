using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace Core.Cache.Manager
{
    public class MemoryCacheManager : ICacheManager
    {
        private static int DefaultCacheDuration => 60;
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();
        private readonly IMemoryCache _cache;

        public MemoryCacheManager(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool Contains(string key)
        {
            return _cache.TryGetValue(key, out object result);
        }

        public void Set<T>(string key, T value)
        {
            _cache.Set(key, value);
        }

        public void Set<T>(string key, T value, int duration)
        {
            if (duration == -1)
            {
                _cache.Set(key, value);
            }
            else
            {
                duration = duration <= 0 ? DefaultCacheDuration : duration;
                _cache.Set(key, value, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now + TimeSpan.FromSeconds(duration),
                    Priority = CacheItemPriority.Low
                });
            }
        }

        public T Get<T>(string key) where T : class
        {
            return _cache.TryGetValue(key, out T result) ? result : default(T);
        }

        public Int64 Count<T>(string key)
        {
            var rslt = _cache.TryGetValue(key, out T result) ? result : default(T);
            return Convert.ToInt64(rslt);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void RemoveRange(string key)
        {
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var collection = field.GetValue(_cache) as ICollection;
            if (collection != null)
            {
                var items = new List<string>();
                foreach (var item in collection)
                {
                    var methodInfo = item.GetType().GetProperty("Key");
                    var val = methodInfo.GetValue(item);
                    items.Add(val.ToString());
                }
                var Keys = items.FindAll(x => x.Contains(key));
                foreach (var item in Keys)
                {
                    _cache.Remove(item);
                }
            }
        }

        public void Reset()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested &&
                _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }
            _resetCacheToken = new CancellationTokenSource();
        }

        public IList<T> FetchDataIList<T>(string key, string countkey, int duration, IEnumerable<T> rs, int skip, int take)
        {
            IList<T> dt;

            if (Contains(key) == false)
            {
                if (countkey == null)
                {
                    dt = rs.AsEnumerable().OfType<T>().ToList<T>();
                    Set<IList<T>>(key, dt, duration);
                }
                else
                {
                    dt = rs.AsEnumerable().OfType<T>().Skip<T>(skip).Take<T>(take).ToList<T>();
                    
                    Set<IList<T>>(key, dt, duration);
                    Set<Int64>(countkey, rs.AsEnumerable().OfType<T>().Count<T>(), duration);
                }
            }
            else
            {
                if (countkey == null)
                {
                    dt = Get<IList<T>>(key).Skip(0).Take(1).ToList();
                }
                else
                {
                    dt = Get<IList<T>>(key).ToList();
                }
            }

            return dt;
        }

        public IEnumerable<T> FetchDataIEnumerable<T>(string key, string countkey, int duration, IEnumerable<T> rs, int skip, int take)
        {
            IEnumerable<T> dt;

            if (Contains(key) == false)
            {
                if (countkey == null)
                {
                    dt = rs.AsEnumerable().OfType<T>().ToList<T>();
                    Set<IEnumerable<T>>(key, dt, duration);
                }
                else
                {
                    dt = rs.AsEnumerable().OfType<T>().Skip<T>(skip).Take<T>(take).ToList<T>();
                    Set<IEnumerable<T>>(key, dt, duration);
                    Set<Int64>(countkey, rs.AsEnumerable().OfType<T>().Count<T>(), duration);
                }
            }
            else
            {
                if (countkey == null)
                {
                    dt = Get<IEnumerable<T>>(key).Skip(0).Take(1).ToList();
                }
                else
                {
                    dt = Get<IEnumerable<T>>(key).ToList();
                }
            }
            return dt;
        }

        public T FetchData<T>(string key, int duration, IEnumerable<T> rs) where T : class
        {
            T dt;

            if (Contains(key) == false)
            {
                dt = rs.AsEnumerable().OfType<T>().SingleOrDefault<T>();
                Set<T>(key, dt, duration);
            }
            else
            {
                dt = _cache.Get<T>(key);
            }

            return dt;
        }

    }

}
