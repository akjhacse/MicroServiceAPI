using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MicroService.Infrastructure.Exceptions;
using MicroService.Infrastructure.Helper;
using MicroService.Infrastructure.Interfaces;

namespace MicroService.Infrastructure.ReadModel {

   [ExcludeFromCodeCoverage]
   public class SmsCacheProvider : ISmsCacheProvider {
      private static readonly StackExchange.Redis.IDatabase Cache;

      static SmsCacheProvider() {
         Cache = RedisConnectorHelper.Connection.GetDatabase();
      }

      public async Task SetCacheAsync<T>(string cacheKey, T value, int cacheTimeoutInMinutes = 0) {
         try {           
            await Cache.StringSetAsync(cacheKey, value.ToString());
            if (cacheTimeoutInMinutes != 0) {
               await Cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(cacheTimeoutInMinutes));
            }
         }
         catch (Exception) {
            throw new CacheException("An error occured while setting the cache.");
         }
      }

      public async Task<T> GetCacheAsync<T>(string cacheKey) where T : class {
         try {
            return (await Cache.StringGetAsync(cacheKey)) as T;
         }
         catch (Exception) {
            throw new CacheException("An error occured while reading the cache.");
         }
      }

      public async Task<bool> KeyExistsAsync(string cacheKey) {
         try {
            return await Cache.KeyExistsAsync(cacheKey);
         }
         catch (Exception) {
            throw new CacheException("An error occured while reading the cache.");
         }
      }
   }
}
