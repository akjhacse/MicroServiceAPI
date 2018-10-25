using System;
using System.Linq;
using System.Threading.Tasks;
using MicroService.Infrastructure.Entities;
using MicroService.Infrastructure.Exceptions;
using MicroService.Infrastructure.Interfaces;

namespace MicroService.Infrastructure.ReadModel {

   public class SmsDao : ISmsDao {
      private readonly IReadModelQuery _readModelQuery;
      private readonly UserProfile _userProfile;
      private readonly ISmsCacheProvider _smsCacheProvider;
      private const string CacheKey = "sms-key";

      public SmsDao(string connectionString, UserProfile userProfile, ISmsCacheProvider smsCacheProvider) : this(new SqlServerReadModelQuery(connectionString), userProfile, smsCacheProvider) {
      }

      public SmsDao(IReadModelQuery readModelQuery, UserProfile userProfile, ISmsCacheProvider smsCacheProvider) {
         _readModelQuery = readModelQuery;
         _userProfile = userProfile;
         _smsCacheProvider = smsCacheProvider;
      }

      public async Task<bool> IsAuthorizedUserAsync(string userName, string password) {
         var sqlQuery = $"SELECT 1 FROM dbo.Accounts WHERE Auth_Id = '{password}' AND UserName = '{userName}'";
         return (await _readModelQuery.QueryAsync<bool>(sqlQuery)).Any();
      }

      public async Task<bool> IsAccountNumberExistsAsync(string accountNumber) {
         var sqlQuery = $@"SELECT 1 FROM dbo.Accounts a
                           JOIN dbo.Phone_Number pn ON pn.Account_Id = a.Id
                           WHERE a.UserName = '{_userProfile.UserName}' AND pn.Number='{accountNumber}'";

         return (await _readModelQuery.QueryAsync<bool>(sqlQuery)).Any();
      }

      public async Task ApplyInBoundCacheRuleAsync(Sms sms) {
         if (sms.Text.TrimEnd('\r', '\n') == "STOP") {
            var key = GetSmsCacheKey(sms);
            await _smsCacheProvider.SetCacheAsync(key, "STOP", 4*60);
         }
      }

      public async Task ApplyOutBoundCacheRuleAsync(Sms sms) {
         if (await _smsCacheProvider.KeyExistsAsync(GetSmsCacheKey(sms))) {
            throw new SmsOutBoundException($"Sms from '{sms.From}' to '{sms.To}' is blocked by STOP request.");
         }

         var fromCountCacheKey = $"{CacheKey}-{sms.From}-count";

         var requestCount = Convert.ToInt16(await _smsCacheProvider.GetCacheAsync<string>(fromCountCacheKey));
         if (requestCount == 0) {
            await _smsCacheProvider.SetCacheAsync(fromCountCacheKey, 1, 24*60);          
         }
         else if (requestCount >= 50) {
            throw new SmsOutBoundException($"Limit reached for from '{sms.From}'.");
         }
         else {
            await _smsCacheProvider.SetCacheAsync(fromCountCacheKey, requestCount + 1);
         }
      }

      private string GetSmsCacheKey(Sms sms) {
         return $"{CacheKey}-{sms.To}-{sms.From}";
      }
   }
}
