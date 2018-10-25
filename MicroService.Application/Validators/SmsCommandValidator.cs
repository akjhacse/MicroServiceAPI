using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MicroService.Application.Commands;
using MicroService.Infrastructure.Interfaces;

namespace MicroService.Application.Validators {

   public abstract class SmsCommandValidator<T> : AbstractValidator<T> where T : SmsCommand {
      private readonly ISmsDao _smsDao;

      protected SmsCommandValidator(ISmsDao smsDao) {
         _smsDao = smsDao;
         RuleFor(x => x.To).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().WithMessage("To is missing.").Length(6, 16).WithMessage("To is invalid.");
         RuleFor(x => x.From).NotEmpty().WithMessage("From is missing.").Length(6, 16).WithMessage("From is invalid.");
         RuleFor(x => x.Text).NotEmpty().WithMessage("Text is missing.");
      }

      protected async Task<bool> IsAccountNumberExists(string accountNumber, CancellationToken cancellationToken) {
         return await _smsDao.IsAccountNumberExistsAsync(accountNumber);
      }
   }
}
