using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using MicroService.Infrastructure.Entities;
using Owin;

namespace MicroService.Api.Provider {

   [ExcludeFromCodeCoverage]
   public static class UserProfileProvider {
      private const string UserProfileKey = "UserProfile";

      public static UserProfile GetUserProfile() {
         object context;
         if (OwinRequestScopeContext.Current != null && OwinRequestScopeContext.Current.Items.TryGetValue(UserProfileKey, out context)) {
            return (UserProfile)context;
         }
         if (HttpContext.Current != null && HttpContext.Current.Items.Contains(UserProfileKey)) {
            return (UserProfile)HttpContext.Current.Items[UserProfileKey];
         }
         return null;
      }

      public static void SetUserProfile(UserProfile context) {
         if (OwinRequestScopeContext.Current != null) {
            OwinRequestScopeContext.Current.Items[UserProfileKey] = context;
         }
         else if (HttpContext.Current != null) {
            HttpContext.Current.Items[UserProfileKey] = context;
         }
         else {
            throw new InvalidOperationException("Cannot set user profile.");
         }
      }

   }
}