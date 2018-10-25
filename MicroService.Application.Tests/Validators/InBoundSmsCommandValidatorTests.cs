﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroService.Application.CommandHandlers;
using MicroService.Application.Commands;
using MicroService.Application.Validators;
using MicroService.Infrastructure.Interfaces;
using Moq;

namespace MicroService.Application.Tests.Validators {

   [TestClass]
   public class InBoundSmsCommandValidatorTests {

      private Mock<ISmsDao> _mockSmsDao;
      private InBoundSmsCommandValidator _validator;

      [TestInitialize]
      public void TestInitialize() {
         _mockSmsDao = new Mock<ISmsDao>(MockBehavior.Strict);
         _validator = new InBoundSmsCommandValidator(_mockSmsDao.Object);
      }

      [TestCleanup]
      public void TestCleanup() {         
         _mockSmsDao.VerifyAll();
      }

      [TestMethod]
      public async Task InBoundSmsCommandValidatorTest_ToIsMissing() {
         //Arrange
         var command = new InBoundSmsCommand {
            From = "12345678",
            Text = "tecxt"
         };


         //Act
         var result = await _validator.ValidateAsync(command, CancellationToken.None);

         //Assert
         Assert.IsFalse(result.IsValid);
         Assert.AreEqual(1, result.Errors.Count);
         Assert.AreEqual("To is missing.", result.Errors.Single().ErrorMessage);
      }

      [TestMethod]
      public async Task InBoundSmsCommandValidatorTest_ToLengthIsSmaller() {
         //Arrange
         var command = new InBoundSmsCommand {
            To = "123",
            From = "12345678",
            Text = "tecxt"
         };
         _mockSmsDao.Setup(x => x.IsAccountNumberExistsAsync(command.To)).ReturnsAsync(true);

         //Act
         var result = await _validator.ValidateAsync(command, CancellationToken.None);

         //Assert
         Assert.IsFalse(result.IsValid);
         Assert.AreEqual(1, result.Errors.Count);
         Assert.AreEqual("To is invalid.", result.Errors.Single().ErrorMessage);
      }

      [TestMethod]
      public async Task InBoundSmsCommandValidatorTest_ToLengthIsLarger() {
         //Arrange
         var command = new InBoundSmsCommand {
            To = "123456789123456789",
            From = "1234567899",
            Text = "tecxt"
         };
         _mockSmsDao.Setup(x => x.IsAccountNumberExistsAsync(command.To)).ReturnsAsync(true);

         //Act
         var result = await _validator.ValidateAsync(command, CancellationToken.None);

         //Assert
         Assert.IsFalse(result.IsValid);
         Assert.AreEqual(1, result.Errors.Count);
         Assert.AreEqual("To is invalid.", result.Errors.Single().ErrorMessage);
      }

      [TestMethod]
      public async Task InBoundSmsCommandValidatorTest_FromIsMissing() {
         //Arrange
         var command = new InBoundSmsCommand {
            To = "12345678",
            Text = "tecxt"
         };
         _mockSmsDao.Setup(x => x.IsAccountNumberExistsAsync(command.To)).ReturnsAsync(true);

         //Act
         var result = await _validator.ValidateAsync(command, CancellationToken.None);

         //Assert
         Assert.IsFalse(result.IsValid);
         Assert.AreEqual(1, result.Errors.Count);
         Assert.AreEqual("From is missing.", result.Errors.Single().ErrorMessage);
      }

      [TestMethod]
      public async Task InBoundSmsCommandValidatorTest_FromLengthIsSmaller() {
         //Arrange
         var command = new InBoundSmsCommand {
            To = "12345678",
            From = "123",
            Text = "tecxt"
         };
         _mockSmsDao.Setup(x => x.IsAccountNumberExistsAsync(command.To)).ReturnsAsync(true);

         //Act
         var result = await _validator.ValidateAsync(command, CancellationToken.None);

         //Assert
         Assert.IsFalse(result.IsValid);
         Assert.AreEqual(1, result.Errors.Count);
         Assert.AreEqual("From is invalid.", result.Errors.Single().ErrorMessage);
      }

      [TestMethod]
      public async Task InBoundSmsCommandValidatorTest_FromLengthIsLarger() {
         //Arrange
         var command = new InBoundSmsCommand {
            To = "123456789",
            From = "12345678991234567899",
            Text = "tecxt"
         };
         _mockSmsDao.Setup(x => x.IsAccountNumberExistsAsync(command.To)).ReturnsAsync(true);

         //Act
         var result = await _validator.ValidateAsync(command, CancellationToken.None);

         //Assert
         Assert.IsFalse(result.IsValid);
         Assert.AreEqual(1, result.Errors.Count);
         Assert.AreEqual("From is invalid.", result.Errors.Single().ErrorMessage);
      }

      [TestMethod]
      public async Task InBoundSmsCommandValidatorTest_TextIsMissing() {
         //Arrange
         var command = new InBoundSmsCommand {
            To = "12345678",
            From = "12345678",
            Text = ""
         };
         _mockSmsDao.Setup(x => x.IsAccountNumberExistsAsync(command.To)).ReturnsAsync(true);

         //Act
         var result = await _validator.ValidateAsync(command, CancellationToken.None);

         //Assert
         Assert.IsFalse(result.IsValid);
         Assert.AreEqual(1, result.Errors.Count);
         Assert.AreEqual("Text is missing.", result.Errors.Single().ErrorMessage);
      }

      [TestMethod]
      public async Task InBoundSmsCommandValidatorTest_ToDoesntExists() {
         //Arrange
         var command = new InBoundSmsCommand {
            To = "12345678",
            From = "12345678",
            Text = "text"
         };
         _mockSmsDao.Setup(x => x.IsAccountNumberExistsAsync(command.To)).ReturnsAsync(false);

         //Act
         var result = await _validator.ValidateAsync(command, CancellationToken.None);

         //Assert
         Assert.IsFalse(result.IsValid);
         Assert.AreEqual(1, result.Errors.Count);
         Assert.AreEqual("To parameter not found.", result.Errors.Single().ErrorMessage);
      }

      [TestMethod]
      public async Task InBoundSmsCommandValidatorTestSuccess() {
         //Arrange
         var command = new InBoundSmsCommand {
            To = "12345678",
            From = "12345678",
            Text = "text"
         };
         _mockSmsDao.Setup(x => x.IsAccountNumberExistsAsync(command.To)).ReturnsAsync(true);

         //Act
         var result = await _validator.ValidateAsync(command, CancellationToken.None);

         //Assert
         Assert.IsTrue(result.IsValid);        
      }
   }
}
