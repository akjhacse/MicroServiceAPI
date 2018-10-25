using System.Threading.Tasks;
using MicroService.Infrastructure.Entities;

namespace MicroService.Infrastructure.Interfaces {

   public interface ISmsDao {

      Task<bool> IsAuthorizedUserAsync(string userName, string password);

      Task<bool> IsAccountNumberExistsAsync(string accountNumber);

      Task ApplyInBoundCacheRuleAsync(Sms sms);

      Task ApplyOutBoundCacheRuleAsync(Sms sms);
   }
}
