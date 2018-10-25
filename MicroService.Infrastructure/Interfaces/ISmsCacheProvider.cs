using System.Threading.Tasks;

namespace MicroService.Infrastructure.Interfaces {

   public interface ISmsCacheProvider {

      Task SetCacheAsync<T>(string cacheKey, T value, int cacheTimeoutInMinutes = 0);

      Task<T> GetCacheAsync<T>(string cacheKey) where T : class;

      Task<bool> KeyExistsAsync(string cacheKey);
   }
}
