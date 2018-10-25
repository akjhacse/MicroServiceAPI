using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MicroService.Application.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroService.Application.Commands;
using MicroService.Infrastructure.Entities;
using Moq;
using MicroService.Infrastructure.Interfaces;

namespace MicroService.Application.Tests.CommandHandlers {

   [TestClass]
   public class InBoundSmsCommandHandlerTests {

      private Mock<IValidator<InBoundSmsCommand>> _mockValidator;
      private InBoundSmsCommandHandler _handler;
      private Mock<ISmsDao> _mockSmsDao;

      [TestInitialize]
      public void TestInitialize() {
         _mockValidator = new Mock<IValidator<InBoundSmsCommand>>(MockBehavior.Strict);
         _mockSmsDao = new Mock<ISmsDao>(MockBehavior.Strict);
         _handler = new InBoundSmsCommandHandler(_mockValidator.Object, _mockSmsDao.Object);
      }

      [TestCleanup]
      public void TestCleanup() {
         _mockValidator.VerifyAll();
         _mockSmsDao.VerifyAll();
      }

      [TestMethod]
      public async Task InBoundSmsCommandHandlerTest_ValidationError() {
         //Arrange
         var command = new InBoundSmsCommand();

         var validationResult = new ValidationResult {
            Errors = {new ValidationFailure("prop", "messgae")}
         };
         _mockValidator.Setup(x => x.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(validationResult);

         //Act
         var result = await _handler.Handle(command, CancellationToken.None);

         //Assert
         Assert.IsNotNull(result);
         Assert.IsFalse(result.IsValid);
         Assert.AreEqual(1, validationResult.Errors.Count);
         Assert.AreEqual(validationResult.Errors.Single().ErrorMessage, result.ValidationResult.Errors.Single().ErrorMessage);
      }

      [TestMethod]
      public async Task InBoundSmsCommandHandlerTest_Success() {
         //Arrange
         var command = new InBoundSmsCommand();

         _mockValidator.Setup(x => x.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
         _mockSmsDao.Setup(x => x.ApplyInBoundCacheRuleAsync(It.IsAny<Sms>())).Returns(Task.CompletedTask);

         //Act
         var result = await _handler.Handle(command, CancellationToken.None);

         //Assert
         Assert.IsNotNull(result);
         Assert.IsTrue(result.IsValid);        
      }
   }
}
