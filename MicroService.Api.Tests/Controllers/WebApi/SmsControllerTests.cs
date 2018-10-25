using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentValidation.Results;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroService.Api.Controllers.WebApi;
using MicroService.Api.Entities;
using Moq;
using MicroService.Application.Commands;

namespace MicroService.Api.Tests.Controllers.WebApi {

   [TestClass]
   public class SmsControllerTests {
      private SmsController _controller;
      private Mock<IMediator> _mockBus;

      [TestInitialize]
      public void TestInitialize() {
         _mockBus = new Mock<IMediator>(MockBehavior.Strict);
         _controller = new SmsController(_mockBus.Object) {Request = new HttpRequestMessage()};
         _controller.Request.SetConfiguration(new HttpConfiguration());
      }

      [TestCleanup]
      public void TestCleanup() {
         _mockBus.VerifyAll();
      }

      [TestMethod]
      public async Task InboundSmsTest_ValidationError() {
         //Arrange
         var request = new SmsCommand();
         var commandResult= new CommandResult {
            ValidationResult = new ValidationResult {
               Errors = { new ValidationFailure("prop", "messgae")}
            }
         };
         _mockBus.Setup(x => x.Send(It.IsAny<InBoundSmsCommand>(), CancellationToken.None)).ReturnsAsync(commandResult);

         //Act
         var response = await _controller.InboundSms(request);
         var result = (await response.Content.ReadAsAsync<WebApiResponse>());

         //Assert
         Assert.IsNotNull(result);
         Assert.AreEqual(1, commandResult.ValidationResult.Errors.Count);
         Assert.AreEqual(commandResult.ValidationResult.Errors.Single().ErrorMessage, result.Error);
      }

      [TestMethod]
      public async Task InboundSmsTest_Success() {
         //Arrange
         var request = new SmsCommand();

         _mockBus.Setup(x => x.Send(It.IsAny<InBoundSmsCommand>(), CancellationToken.None)).ReturnsAsync(new CommandResult());

         //Act
         var response = await _controller.InboundSms(request);
         var result = (await response.Content.ReadAsAsync<WebApiResponse>());

         //Assert
         Assert.IsNotNull(result);
        
         Assert.AreEqual("inbound sms ok.", result.Message);
      }

      [TestMethod]
      public async Task OutboundSmsTest_ValidationError() {
         //Arrange
         var request = new SmsCommand();
         var commandResult = new CommandResult {
            ValidationResult = new ValidationResult {
               Errors = { new ValidationFailure("prop", "messgae") }
            }
         };
         _mockBus.Setup(x => x.Send(It.IsAny<OutBoundSmsCommand>(), CancellationToken.None)).ReturnsAsync(commandResult);

         //Act
         var response = await _controller.OutboundSms(request);
         var result = (await response.Content.ReadAsAsync<WebApiResponse>());

         //Assert
         Assert.IsNotNull(result);
         Assert.AreEqual(1, commandResult.ValidationResult.Errors.Count);
         Assert.AreEqual(commandResult.ValidationResult.Errors.Single().ErrorMessage, result.Error);
      }

      [TestMethod]
      public async Task OutboundSmsTest_Success() {
         //Arrange
         var request = new SmsCommand();

         _mockBus.Setup(x => x.Send(It.IsAny<OutBoundSmsCommand>(), CancellationToken.None)).ReturnsAsync(new CommandResult());

         //Act
         var response = await _controller.OutboundSms(request);
         var result = (await response.Content.ReadAsAsync<WebApiResponse>());

         //Assert
         Assert.IsNotNull(result);

         Assert.AreEqual("outbound sms ok.", result.Message);
      }
   }
}
