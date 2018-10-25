using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using StackExchange.Redis;

namespace MicroService.Infrastructure.Helper {

   [ExcludeFromCodeCoverage]
   public class RedisConnectorHelper {

      private static readonly Lazy<ConnectionMultiplexer> LazyConnection;
      private static readonly string RedisCacheServer = ConfigurationManager.AppSettings["RedisCacheServer"];

      static RedisConnectorHelper() {
         LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(RedisCacheServer));
      }

      public static ConnectionMultiplexer Connection => LazyConnection.Value;
   }
}
