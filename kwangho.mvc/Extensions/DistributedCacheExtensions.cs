using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.Caching.Distributed
{
    /// <summary>
    /// 분산 캐시 확장 메서드
    /// </summary>
    public static class DistributedCacheExtensions
    {
        private static JsonSerializerOptions serializerOptions = new()
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private static DistributedCacheEntryOptions GetdefaultCacheOption()
        {
            return new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        }

        /// <summary>
        /// 캐시에 저장
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value)
        {
            return SetAsync(cache, key, value, GetdefaultCacheOption());
        }

        /// <summary>
        /// 캐시에 저장
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, serializerOptions));
            return cache.SetAsync(key, bytes, options);
        }

        /// <summary>
        /// 캐시에서 가져오기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key)
        {
            T? value = default;

            var val = await cache.GetAsync(key);
            if (val != null)
                value = JsonSerializer.Deserialize<T>(val, serializerOptions);

            return value;
        }

        /// <summary>
        /// 캐시에서 가져오기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T? value)
        {
            value = default;

            var val = cache.Get(key);
            if (val == null)
                return false;

            value = JsonSerializer.Deserialize<T>(val, serializerOptions);
            return true;
        }

        /// <summary>
        /// 캐시에서 가져오기
        /// 없으면 새로운 값을 가져와서 캐시에 저장
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="task"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<T?> GetOrSetAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> task, DistributedCacheEntryOptions? options = null)
        {
            options ??= GetdefaultCacheOption();

            if (cache.TryGetValue(key, out T? value) && value is not null)
            {
                return value;
            }

            value = await task();

            if (value is not null)
            {
                await cache.SetAsync<T>(key, value, options);
            }

            return value;
        }
    }
}