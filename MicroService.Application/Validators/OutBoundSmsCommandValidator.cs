using FluentValidation;
using MicroService.Application.Commands;
using MicroService.Infrastructure.Interfaces;

namespace MicroService.Application.Validators {

   public class OutBoundSmsCommandValidator : SmsCommandValidator<OutBoundSmsCommand> {

      public OutBoundSmsCommandValidator(ISmsDao smsDao) : base(smsDao) {
         RuleFor(x => x.From).MustAsync(IsAccountNumberExists).WithName("From").WithMessage("From parameter not found.").When(x => !string.IsNullOrWhiteSpace(x.From));
      }
   }
}
