using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Web.Http.ExceptionHandling;

namespace MicroService.Api.Middleware {

   /// <summary>
   /// Custome exception handler. It will override the default exception handling
   /// </summary>
   [ExcludeFromCodeCoverage]
   public class WebApiExceptionHandler : ExceptionHandler {

      public override void Handle(ExceptionHandlerContext context) {
         var info = ExceptionDispatchInfo.Capture(context.Exception);
         info.Throw();
      }
   }
}