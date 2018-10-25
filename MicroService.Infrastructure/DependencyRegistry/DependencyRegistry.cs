using System.Configuration;
using MicroService.Infrastructure.Interfaces;
using MicroService.Infrastructure.ReadModel;
using StructureMap;

namespace MicroService.Infrastructure.DependencyRegistry {
   public class DependencyRegistry : Registry {

      private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["SMSDbConnection"].ConnectionString;

      public DependencyRegistry() {
         For<ISmsDao>().Use<SmsDao>().Ctor<string>("connectionString").Is(ConnectionString);
         For<ISmsCacheProvider>().Use<SmsCacheProvider>();
      }
   }
}
