using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MicroService.Api.Entities;
using MicroService.Api.Provider;
using MicroService.Infrastructure.Entities;

namespace MicroService.Api.Controllers {

   public abstract class ApiControllerBase : ApiController {

      protected async Task<HttpResponseMessage> ErrorAsync(string errorMessge) {
         return await ResponseAsync(errorMessge, HttpStatusCode.BadRequest);
      }

      protected async Task<HttpResponseMessage> SuccessAsync(string messge) {
         return await ResponseAsync("", HttpStatusCode.OK, messge);
      }

      private async Task<HttpResponseMessage> ResponseAsync(string errorMessage, HttpStatusCode httpStatusCode, string message = "") {
         var result = new WebApiResponse { Message = message, Error = errorMessage };
         var response = Request.CreateResponse(httpStatusCode, result);

         return await Task.FromResult(response);
      }
   }
}