using FluentValidation;
using MicroService.Application.Commands;
using MicroService.Infrastructure.Interfaces;

namespace MicroService.Application.Validators {

   public class InBoundSmsCommandValidator : SmsCommandValidator<InBoundSmsCommand> {

      public InBoundSmsCommandValidator(ISmsDao smsDao) : base(smsDao){
         RuleFor(x => x.To).MustAsync(IsAccountNumberExists).WithName("To").WithMessage("To parameter not found.").When(x => !string.IsNullOrWhiteSpace(x.To));
      }
   }
}
