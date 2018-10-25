using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MicroService.Infrastructure.Entities;
using MicroService.Infrastructure.Interfaces;


namespace MicroService.Validation.Validators {

   public class SmsValidator : AbstractValidator<Sms> {

      private readonly ISmsDao _smsDao;

      public SmsValidator(ISmsDao smsDao) {
         _smsDao = smsDao;

         RuleSet("Required", () => {
            RuleFor(x => x.To).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().WithMessage("To is missing.").Length(6, 16).WithMessage("To is invalid.");
            RuleFor(x => x.From).NotEmpty().WithMessage("From is missing.").Length(6, 16).WithMessage("From is invalid.");
            RuleFor(x => x.Text).NotEmpty().WithMessage("Text is missing.");
         });

         RuleSet("InBound", () => {
            RuleFor(x => x.To).MustAsync(IsAccountNumberExists).WithName("To").WithMessage("To parameter not found.").When(x => !string.IsNullOrWhiteSpace(x.To));
         });
         RuleSet("OutBound", () => {
            RuleFor(x => x.From).MustAsync(IsAccountNumberExists).WithName("From").WithMessage("From parameter not found.").When(x => !string.IsNullOrWhiteSpace(x.From));
         });
      }

      private async Task<bool> IsAccountNumberExists(string toAccountNumber, CancellationToken cancellationToken) {
         return await _smsDao.IsAccountNumberExistsAsync(toAccountNumber);
      }
   }
}
