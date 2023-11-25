using Entity;
using Infrastructure.Repositories;
using Infrastructure;
using Moq;
using FinurligaFinanserWebAPI.Controllers;
using Moq.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FinurligaFinanserWebAPI.Tests.Controllers
{
    public class UserAccountControllerTests
    {
        //private List<UserAccount> _userAccounts;
        
        private Mock<IUserAccountRepository> _mockRepository;
        private Mock<ILogger> _mockLogger;

        private UserAccountController _sut;

        [SetUp]
        public void Setup()
        {
            //_userAccounts = SeedUserAccounts(10);

            _mockRepository = new Mock<IUserAccountRepository>();

            _mockLogger = new Mock<ILogger>();

            _sut = new UserAccountController( _mockRepository.Object, _mockLogger.Object);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(2)]
        [TestCase(4)]
        public async Task GetAll_ShouldReturnOkResultAndListOfUserAccount(int take)
        {
            var userAccounts = take > 0 ? SeedUserAccounts(take) : new List<UserAccount>();

            _mockRepository.Setup(x => x.GetAllUserAccountsAsync(take)).ReturnsAsync(userAccounts);

            var result = await _sut.GetAll(take);

            Assert.That(result.Result, Is.InstanceOf(typeof(OkObjectResult)));

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.That(okResult.Value, Is.InstanceOf(typeof(List<UserAccount>)));

            var resultValue = (List<UserAccount>)okResult.Value;
            Assert.That(resultValue, Is.Not.Null);
            Assert.That(userAccounts.Count, Is.EqualTo(resultValue.Count));
        }

        private List<UserAccount> SeedUserAccounts(int count)
        {
            var userAccounts = new List<UserAccount>();

            for (int i = 0; i < count; i++)
            {
                userAccounts.Add(new UserAccount($"UserAccount{i}", $"FirstName{i}", $"LastName{i}", new byte[32], $"hash{i}"));
            }

            return userAccounts;
        }
    }
}
