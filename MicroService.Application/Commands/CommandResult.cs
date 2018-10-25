using FluentValidation.Results;

namespace MicroService.Application.Commands {

   public class CommandResult {

      /// <summary>
      /// Returns false if the command validation failed; otherwise, returns true. 
      /// </summary>
      public bool IsValid => ValidationResult?.IsValid ?? true;

      public ValidationResult ValidationResult { get; set; }

   }
}
