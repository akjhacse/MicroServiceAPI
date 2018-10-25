using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace MicroService.Api.Exceptions {

   [ExcludeFromCodeCoverage]
   public sealed class AuthorizationException : Exception {

      public AuthorizationException(string message) : base(message) {

      }
   }
}