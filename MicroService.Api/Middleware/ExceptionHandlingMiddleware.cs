using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;
using MicroService.Api.Entities;
using MicroService.Api.Exceptions;
using MicroService.Infrastructure.Exceptions;
using System.Web.Script.Serialization;

namespace MicroService.Api.Middleware {

   [ExcludeFromCodeCoverage]
   public class ExceptionHandlingMiddleware : OwinMiddleware {

      public ExceptionHandlingMiddleware(OwinMiddleware next) : base(next) {
      }

      public override async Task Invoke(IOwinContext context) {
         using (var rewindableRequestBody = new MemoryStream()) {
            var originalBody = context.Request.Body;

            try {
               // Swap the request body in place, so we can read it again when an exception is thrown. 
               await context.Request.Body.CopyToAsync(rewindableRequestBody);
               context.Request.Body = rewindableRequestBody;
               context.Request.Body.Position = 0;

               await Next.Invoke(context);
            }
            catch (Exception exception) {
               await HandleException(context, exception);
            }
            finally {
               // Ensure that the original body gets disposed, so it doesn't leak.
               context.Request.Body = originalBody;
            }
         }
      }

      private async Task HandleException(IOwinContext context, Exception exception) {
         var result = new WebApiResponse { Message = exception?.Message };

         if (exception is AuthorizationException) {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
         }
         else if (exception is SmsOutBoundException) {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
         }
         else {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            result.Error = "unknown faliure.";
         }
         string content = GetContent(result);
         context.Response.ContentLength = content.Length;
         context.Response.ContentType = context.Request.ContentType ?? "application/json";
         await context.Response.WriteAsync(content);
      }

      private string GetContent(WebApiResponse result) {
         return new JavaScriptSerializer().Serialize(result);
      }
   }
}