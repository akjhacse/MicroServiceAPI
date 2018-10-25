using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using MicroService.Api.App_Start;

namespace MicroService.Api.DependencyResolution {
   public class StructureMapValidatorFactory : ValidatorFactoryBase {

      public override IValidator CreateInstance(Type validatorType) {
         return StructuremapMvc.StructureMapDependencyScope.Container.TryGetInstance(validatorType) as IValidator;
      }
   }
}