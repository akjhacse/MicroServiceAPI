using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin;
using MicroService.Api.App_Start;
using MicroService.Api.DependencyResolution;
using MicroService.Api.Middleware;
using MicroService.Infrastructure.Interfaces;
using Owin;

[assembly: OwinStartup(typeof(MicroService.Api.Startup))]

namespace MicroService.Api {

   public class Startup {

      public void Configuration(IAppBuilder app) {
         AreaRegistration.RegisterAllAreas();
         GlobalConfiguration.Configure(WebApiConfig.Register);
         FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
         RouteConfig.RegisterRoutes(RouteTable.Routes);
         BundleConfig.RegisterBundles(BundleTable.Bundles);

         var container = StructuremapMvc.StructureMapDependencyScope.Container;
         var config = new HttpConfiguration { DependencyResolver = new StructureMapWebApiDependencyResolver(container) };
         WebApiConfig.Register(config);

         // Enable OwinRequstScopeContext (similar to HttpContext.Current)
         app.UseRequestScopeContext();
         app.Use(typeof(ExceptionHandlingMiddleware));
         app.Use(typeof(AuthorizationMiddleware), StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<ISmsDao>());
         app.UseWebApi(config);
      }
   }
}
