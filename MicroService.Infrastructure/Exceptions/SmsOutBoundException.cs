using System;
using System.Diagnostics.CodeAnalysis;

namespace MicroService.Infrastructure.Exceptions {

   [ExcludeFromCodeCoverage]
   public sealed class SmsOutBoundException : Exception {

      public SmsOutBoundException(string message) : base(message) {
      }

   }
}
