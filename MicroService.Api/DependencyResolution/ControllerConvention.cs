using StructureMap;
using StructureMap.Graph.Scanning;

namespace MicroService.Api.DependencyResolution {
   using System.Web.Mvc;
   using StructureMap.Graph;
   using StructureMap.Pipeline;
   using StructureMap.TypeRules;

   public class ControllerConvention : IRegistrationConvention {

      public void ScanTypes(TypeSet types, Registry registry) {
         foreach (var type in types.AllTypes()) {
            if (type.CanBeCastTo<Controller>() && !type.IsAbstract) {
               registry.For(type).LifecycleIs(new UniquePerRequestLifecycle());
            }
         }
      }
   }
}