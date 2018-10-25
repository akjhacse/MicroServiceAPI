using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MicroService.Application.Commands {

   public class SmsCommand : IRequest<CommandResult> {

      public string To { get; set; }

      public string From { get; set; }

      public string Text { get; set; }
   }
}
