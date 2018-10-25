using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MicroService.Application.Commands;
using MicroService.Application.Extensions;
using MicroService.Infrastructure.Entities;
using MicroService.Infrastructure.Interfaces;

namespace MicroService.Application.CommandHandlers {

   public class OutBoundSmsCommandHandler : IRequestHandler<OutBoundSmsCommand, CommandResult> {
      private readonly IValidator<OutBoundSmsCommand> _validator;
      private readonly ISmsDao _smsDao;

      public OutBoundSmsCommandHandler(IValidator<OutBoundSmsCommand> validator, ISmsDao smsDao) {
         _validator = validator;
         _smsDao = smsDao;
      }

      public async Task<CommandResult> Handle(OutBoundSmsCommand command, CancellationToken cancellationToken) {
         // Validate the command
         var commandResult = new CommandResult {
            ValidationResult = await _validator.ValidateAsync(command, cancellationToken)
         };

         if (!commandResult.IsValid) {
            return commandResult;
         }

         //Apply rules
         var sms = new Sms();
         sms.Update(command);
         await _smsDao.ApplyOutBoundCacheRuleAsync(sms);

         return commandResult;
      }
   }
}
