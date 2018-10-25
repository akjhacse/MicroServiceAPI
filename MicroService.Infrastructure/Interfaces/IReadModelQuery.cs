using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroService.Infrastructure.Interfaces {

   public interface IReadModelQuery {

      Task<IEnumerable<T>> QueryAsync<T>(string sqlQuery);
   }
}
