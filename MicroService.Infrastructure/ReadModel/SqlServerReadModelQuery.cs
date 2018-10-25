using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Dapper;
using MicroService.Infrastructure.Interfaces;

namespace MicroService.Infrastructure.ReadModel {

   [ExcludeFromCodeCoverage]
   public class SqlServerReadModelQuery : IReadModelQuery {
      private readonly string _connectionString;

      public SqlServerReadModelQuery(string connectionString) {
         _connectionString = connectionString;
      }

      private DbConnection GetConnection() {
         return new SqlConnection(_connectionString);
      }

      public async Task<IEnumerable<T>> QueryAsync<T>(string sqlQuery) {
         using (var conn = GetConnection()) {
            await conn.OpenAsync().ConfigureAwait(false);
            return await conn.QueryAsync<T>(sqlQuery);
         }
      }
   }
}
