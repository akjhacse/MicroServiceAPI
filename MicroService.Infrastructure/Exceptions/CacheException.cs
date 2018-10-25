using System;
using System.Diagnostics.CodeAnalysis;

namespace MicroService.Infrastructure.Exceptions {

   [ExcludeFromCodeCoverage]
   public sealed class CacheException : Exception{

      public CacheException(string message) : base(message) {

      }
   }
}
