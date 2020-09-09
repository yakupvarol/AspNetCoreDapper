using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Core.Cache.Manager
{
    public class RedisCacheManager : ICacheManager
    {
        private static int DefaultCacheDuration => 60;
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();
        protected IDistributedCache _cache;

        public RedisCacheManager(IDistributedCache cache)
        {
            _cache = cache;
        }

        public bool Contains(string key)
        {
            var result = false;
            var fromCache = _cache.GetString(key);
            if (fromCache != null)
            {
                result = true;
            }
            return result;
        }

        public void Set<T>(string key, T value)
        {
            string _value = JsonConvert.SerializeObject(value);
            _cache.Set(key, Encoding.UTF8.GetBytes(_value));
        }

        public void Set<T>(string key, T value, int duration)
        {
            string _value = JsonConvert.SerializeObject(value);
            if (duration == -1)
            {
                _cache.Set(key, Encoding.UTF8.GetBytes(_value));
            }
            else
            {
                duration = duration <= 0 ? DefaultCacheDuration : duration;
                _cache.Set(key, Encoding.UTF8.GetBytes(_value), new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now + TimeSpan.FromSeconds(duration)
                });
            }
        }

        public T Get<T>(string key) where T : class
        {
            var fromCache = _cache.Get(key);
            if (fromCache == null)
            {
                return null;
            }

            var str = Encoding.UTF8.GetString(fromCache);
            if (typeof(T) == typeof(string))
            {
                return str as T;
            }

            return JsonConvert.DeserializeObject<T>(str);
        }

        public Int64 Count<T>(string key)
        {
            var fromCache = _cache.Get(key);
            if (fromCache == null)
            {
                return 0;
            }

            return Convert.ToInt64(Encoding.UTF8.GetString(fromCache));
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
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
                dt = Get<T>(key);
            }

            return dt;
        }

        public void RemoveRange(string key)
        {
            throw new NotImplementedException();
        }
    }
}
