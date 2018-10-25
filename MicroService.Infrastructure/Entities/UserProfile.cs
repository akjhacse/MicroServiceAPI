namespace MicroService.Infrastructure.Entities {

   public class UserProfile {

      public UserProfile(string userName) {
         UserName = userName;
      }

      public string UserName { get; }
   }
}
