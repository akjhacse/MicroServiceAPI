using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FluentValidation.Results;
using MediatR;
using MicroService.Application.Commands;

namespace MicroService.Api.Controllers.WebApi {

    public class SmsController : ApiControllerBase {
       private readonly IMediator _bus;

       public SmsController(IMediator bus) {
          _bus = bus;
       }

       [HttpPost]
       [Route("api/inbound/sms")]
       public async Task<HttpResponseMessage> InboundSms(SmsCommand request) {
          //validate the request.
          var command = new InBoundSmsCommand {
             To = request.To,
             From = request.From,
             Text = request.Text
          };
          var result = await _bus.Send(command);

          if (!result.IsValid) {
             return await ErrorAsync(result.ValidationResult);
          }

          return await SuccessAsync("inbound sms ok.");
       }

       [HttpPost]
       [Route("api/outbound/sms")]
       public async Task<HttpResponseMessage> OutboundSms(SmsCommand request) {
          //validate the request.
          var command = new OutBoundSmsCommand {
             To = request.To,
             From = request.From,
             Text = request.Text
          };

          var result = await _bus.Send(command);

          if (!result.IsValid) {
             return await ErrorAsync(result.ValidationResult);
          }

          return await SuccessAsync("outbound sms ok.");
       }

       private async Task<HttpResponseMessage> ErrorAsync(ValidationResult validationResult) {
          return await ErrorAsync(string.Join(",", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
       }
    }
}
