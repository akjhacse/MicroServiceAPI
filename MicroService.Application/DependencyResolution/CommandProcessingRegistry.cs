using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using StructureMap;

namespace MicroService.Application.DependencyResolution {
   public class CommandProcessingRegistry : Registry{
      public CommandProcessingRegistry() {
         Scan(scan => {
            scan.AssemblyContainingType<IMediator>();
            scan.AssemblyContainingType<CommandProcessingRegistry>();
            scan.ConnectImplementationsToTypesClosing(typeof(IValidator<>));
            scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
            scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
            scan.WithDefaultConventions();
         });
         For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
      }
   }
}
