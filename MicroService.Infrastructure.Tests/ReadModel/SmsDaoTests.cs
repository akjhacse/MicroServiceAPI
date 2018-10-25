using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MicroService.Infrastructure.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroService.Infrastructure.Entities;
using MicroService.Infrastructure.Exceptions;
using MicroService.Infrastructure.ReadModel;
using Moq;

namespace MicroService.Infrastructure.Tests.ReadModel {

   [TestClass]
   public class SmsDaoTests {
      private ISmsDao _smsDao;
      private Mock<IReadModelQuery> _mockReadModelQuery;
      private Mock<ISmsCacheProvider> _mockSmsCacheProvider;

      [TestInitialize]
      public void Testinitialize() {
         _mockReadModelQuery = new Mock<IReadModelQuery>(MockBehavior.Strict);
         _mockSmsCacheProvider = new Mock<ISmsCacheProvider>(MockBehavior.Strict);
         _smsDao = new SmsDao(_mockReadModelQuery.Object, new UserProfile("akjha"), _mockSmsCacheProvider.Object);
      }

      [TestCleanup]
      public void TestCleanup() {
         _mockReadModelQuery.VerifyAll();
         _mockSmsCacheProvider.VerifyAll();
      }

      [TestMethod]
      public async Task SmsDaoTest_IsAuthorizedUserAsync_False() {
         //Arrange
         const string userName = "akjha";
         const string passwrord = "testPassword";

         _mockReadModelQuery.Setup(x => x.QueryAsync<bool>(It.IsAny<string>())).ReturnsAsync(new List<bool>());

         //Act
         var result = await _smsDao.IsAuthorizedUserAsync(userName, passwrord);

         //Assert
         Assert.IsFalse(result);
      }

      [TestMethod]
      public async Task SmsDaoTest_IsAuthorizedUserAsync_true() {
         //Arrange
         const string userName = "akjha";
         const string passwrord = "testPassword";

         _mockReadModelQuery.Setup(x => x.QueryAsync<bool>(It.IsAny<string>())).ReturnsAsync(new List<bool>{true, false});

         //Act
         var result = await _smsDao.IsAuthorizedUserAsync(userName, passwrord);

         //Assert
         Assert.IsTrue(result);
      }

      [TestMethod]
      public async Task SmsDaoTest_IsAccountNumberExistsAsync_False() {
         //Arrange
         const string accountNumber = "123345";

         _mockReadModelQuery.Setup(x => x.QueryAsync<bool>(It.IsAny<string>())).ReturnsAsync(new List<bool>());

         //Act
         var result = await _smsDao.IsAccountNumberExistsAsync(accountNumber);

         //Assert
         Assert.IsFalse(result);
      }

      [TestMethod]
      public async Task SmsDaoTest_IsAccountNumberExistsAsync_True() {
         //Arrange
         const string accountNumber = "123345";

         _mockReadModelQuery.Setup(x => x.QueryAsync<bool>(It.IsAny<string>())).ReturnsAsync(new List<bool> { true, false });

         //Act
         var result = await _smsDao.IsAccountNumberExistsAsync(accountNumber);

         //Assert
         Assert.IsTrue(result);
      }

      [TestMethod]
      public async Task SmsDaoTest_ApplyInBoundCacheRuleAsync_CacheSet_WithSTop() {
         //Arrange
         var sms = new Sms {
            Text = "STOP"
         };

         _mockSmsCacheProvider.Setup(x => x.SetCacheAsync(It.IsAny<string>(), sms.Text, It.IsAny<int>())).Returns(Task.CompletedTask);

         //Act
         await _smsDao.ApplyInBoundCacheRuleAsync(sms);
      }

      [TestMethod]
      public async Task SmsDaoTest_ApplyInBoundCacheRuleAsync_CacheSet_WithSTop_r() {
         //Arrange
         var sms = new Sms {
            Text = "STOP\r"
         };

         _mockSmsCacheProvider.Setup(x => x.SetCacheAsync(It.IsAny<string>(), "STOP", It.IsAny<int>())).Returns(Task.CompletedTask);

         //Act
         await _smsDao.ApplyInBoundCacheRuleAsync(sms);
      }

      [TestMethod]
      public async Task SmsDaoTest_ApplyInBoundCacheRuleAsync_CacheSet_WithSTop_n() {
         //Arrange
         var sms = new Sms {
            Text = "STOP\n"
         };

         _mockSmsCacheProvider.Setup(x => x.SetCacheAsync(It.IsAny<string>(), "STOP", It.IsAny<int>())).Returns(Task.CompletedTask);

         //Act
         await _smsDao.ApplyInBoundCacheRuleAsync(sms);
      }

      [TestMethod]
      public async Task SmsDaoTest_ApplyInBoundCacheRuleAsync_CacheSet_WithSTop_rn() {
         //Arrange
         var sms = new Sms {
            Text = "STOP\r\n"
         };

         _mockSmsCacheProvider.Setup(x => x.SetCacheAsync(It.IsAny<string>(), "STOP", It.IsAny<int>())).Returns(Task.CompletedTask);

         //Act
         await _smsDao.ApplyInBoundCacheRuleAsync(sms);
      }

      [TestMethod]
      public async Task SmsDaoTest_ApplyInBoundCacheRuleAsync_CacheNotSet() {
         //Arrange
         var sms = new Sms {
            Text = "STOP\r\n123"
         };

         //Act
         await _smsDao.ApplyInBoundCacheRuleAsync(sms);
      }

      [TestMethod]
      public void SmsDaoTest_ApplyOutBoundCacheRuleAsync_StopRequestExistInCache() {
         //Arrange
         var sms = new Sms {
            Text = "STOP\r\n123"
         };

         _mockSmsCacheProvider.Setup(x => x.KeyExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

         //Act
         Func<Task> fun = async () => await _smsDao.ApplyOutBoundCacheRuleAsync(sms);

         //Assert
         fun.Should().Throw<SmsOutBoundException>() .WithMessage($"Sms from '{sms.From}' to '{sms.To}' is blocked by STOP request.");
      }

      [TestMethod]
      public async Task SmsDaoTest_ApplyOutBoundCacheRuleAsync_FirstRequest_SetCache() {
         //Arrange
         var sms = new Sms {
            Text = "STOP\r\n123"
         };

         _mockSmsCacheProvider.Setup(x => x.KeyExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
         _mockSmsCacheProvider.Setup(x => x.GetCacheAsync<string>(It.IsAny<string>())).ReturnsAsync("0");
         _mockSmsCacheProvider.Setup(x => x.SetCacheAsync(It.IsAny<string>(), 1, 24*60)).Returns(Task.CompletedTask);

         //Act
         await _smsDao.ApplyOutBoundCacheRuleAsync(sms);
      }

      [TestMethod]
      public async Task SmsDaoTest_ApplyOutBoundCacheRuleAsync_RequestGreaterThanOneLessThan50_SetCache() {
         //Arrange
         var sms = new Sms {
            Text = "STOP\r\n123"
         };

         _mockSmsCacheProvider.Setup(x => x.KeyExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
         _mockSmsCacheProvider.Setup(x => x.GetCacheAsync<string>(It.IsAny<string>())).ReturnsAsync("3");
         _mockSmsCacheProvider.Setup(x => x.SetCacheAsync(It.IsAny<string>(), 4, 0)).Returns(Task.CompletedTask);

         //Act
         await _smsDao.ApplyOutBoundCacheRuleAsync(sms);
      }

      [TestMethod]
      public void SmsDaoTest_ApplyOutBoundCacheRuleAsync_RequestCount50() {
         //Arrange
         var sms = new Sms {
            Text = "STOP\r\n123"
         };

         _mockSmsCacheProvider.Setup(x => x.KeyExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
         _mockSmsCacheProvider.Setup(x => x.GetCacheAsync<string>(It.IsAny<string>())).ReturnsAsync("50");

         //Act
         Func<Task> fun = async () => await _smsDao.ApplyOutBoundCacheRuleAsync(sms);

         //Assert
         fun.Should().Throw<SmsOutBoundException>().WithMessage($"Limit reached for from '{sms.From}'.");
      }

   }
}
