using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using MicroService.Api.Exceptions;
using MicroService.Api.Provider;
using MicroService.Infrastructure.Entities;
using MicroService.Infrastructure.Interfaces;

namespace MicroService.Api.Middleware {

   [ExcludeFromCodeCoverage]
   public class AuthorizationMiddleware : OwinMiddleware {

      private readonly ISmsDao _smsDao;

      public AuthorizationMiddleware(OwinMiddleware next, ISmsDao smsDao) : base(next) {
         _smsDao = smsDao;
      }

      public override async Task Invoke(IOwinContext context) {
         if (context.Request.Path.ToString().StartsWith("/api", StringComparison.OrdinalIgnoreCase)) {
            await ValidateUser(context);
         }
         await Next.Invoke(context);
      }

      private async Task ValidateUser(IOwinContext context) {
         string authHeader = context.Request.Headers["Authorization"];

         if (authHeader != null && authHeader.StartsWith("Basic")) {
            string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
            var encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int seperatorIndex = usernamePassword.IndexOf(':');

            var username = usernamePassword.Substring(0, seperatorIndex);
            var password = usernamePassword.Substring(seperatorIndex + 1);
            if (!await _smsDao.IsAuthorizedUserAsync(username, password)) {
               throw new AuthorizationException($"User '{username}' is not authorized to access the api.");
            }
            UserProfileProvider.SetUserProfile(new UserProfile(username));
         }
         else {
            throw new AuthorizationException("The authorization header is either empty or is not Basic.");
         }
      }
   }
}