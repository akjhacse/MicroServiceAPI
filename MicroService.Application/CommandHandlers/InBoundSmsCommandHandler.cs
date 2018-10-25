using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MicroService.Application.Commands;
using MicroService.Application.Extensions;
using MicroService.Infrastructure.Entities;
using MicroService.Infrastructure.Interfaces;

namespace MicroService.Application.CommandHandlers {

   public class InBoundSmsCommandHandler : IRequestHandler<InBoundSmsCommand, CommandResult> {
      private readonly IValidator<InBoundSmsCommand> _validator;
      private readonly ISmsDao _smsDao;

      public InBoundSmsCommandHandler(IValidator<InBoundSmsCommand> validator, ISmsDao smsDao) {
         _validator = validator;
         _smsDao = smsDao;
      }

      public async Task<CommandResult> Handle(InBoundSmsCommand command, CancellationToken cancellationToken) {
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
         await _smsDao.ApplyInBoundCacheRuleAsync(sms);

         return commandResult;
      }    
   }
}
