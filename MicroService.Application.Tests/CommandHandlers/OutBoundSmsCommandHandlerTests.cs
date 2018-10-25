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
   public class OutBoundSmsCommandHandlerTests {

      private Mock<IValidator<OutBoundSmsCommand>> _mockValidator;
      private OutBoundSmsCommandHandler _handler;
      private Mock<ISmsDao> _mockSmsDao;

      [TestInitialize]
      public void TestInitialize() {
         _mockValidator = new Mock<IValidator<OutBoundSmsCommand>>(MockBehavior.Strict);
         _mockSmsDao = new Mock<ISmsDao>(MockBehavior.Strict);
         _handler = new OutBoundSmsCommandHandler(_mockValidator.Object, _mockSmsDao.Object);
      }

      [TestCleanup]
      public void TestCleanup() {
         _mockValidator.VerifyAll();
         _mockSmsDao.VerifyAll();
      }

      [TestMethod]
      public async Task OutBoundSmsCommandHandlerTest_ValidationError() {
         //Arrange
         var command = new OutBoundSmsCommand();

         var validationResult = new ValidationResult {
            Errors = { new ValidationFailure("prop", "messgae") }
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
      public async Task OutBoundSmsCommandHandlerTest_Success() {
         //Arrange
         var command = new OutBoundSmsCommand();

         _mockValidator.Setup(x => x.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
         _mockSmsDao.Setup(x => x.ApplyOutBoundCacheRuleAsync(It.IsAny<Sms>())).Returns(Task.CompletedTask);

         //Act
         var result = await _handler.Handle(command, CancellationToken.None);

         //Assert
         Assert.IsNotNull(result);
         Assert.IsTrue(result.IsValid);
      }
   }
}
