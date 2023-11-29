using Entity;
using Infrastructure.Repositories;
using Moq;
using FinurligaFinanserWebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FinurligaFinanserWebAPI.DtoModels.UserAccountDTOs;

namespace FinurligaFinanserWebAPI.Tests.Controllers
{
    public class UserAccountControllerTests
    {
        //private List<UserAccount> _userAccounts;
        
        private Mock<IUserAccountRepository> _mockRepository;
        private Mock<ILogger<UserAccountController>> _mockLogger;
        private Mock<IMapper> _mockMapper;

        private UserAccountController _sut;

        [SetUp]
        public void Setup()
        {
            //_userAccounts = SeedUserAccounts(10);

            _mockRepository = new Mock<IUserAccountRepository>();

            _mockLogger = new Mock<ILogger<UserAccountController>>();

            _mockMapper = new Mock<IMapper>();

            _sut = new UserAccountController( _mockRepository.Object, _mockLogger.Object, _mockMapper.Object);
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
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf(typeof(List<UserAccount>)));

            var resultValue = (List<UserAccount>)okResult.Value;
            Assert.Multiple(() =>
            {
                Assert.That(resultValue, Is.Not.Null);
                if (resultValue is not null) Assert.That(userAccounts, Has.Count.EqualTo(resultValue.Count));
            });
        }

        [Test]
        [TestCase("", "ValidPassword1!", typeof(BadRequestObjectResult))]
        [TestCase(null, "ValidPassword1!", typeof(BadRequestObjectResult))]
        [TestCase("ValidUsername", "", typeof(BadRequestObjectResult))]
        [TestCase("ValidUsername", null, typeof(BadRequestObjectResult))]
        public async Task Login_UserNameOrPasswordIsNullOrEmpty_ReturnBadRequest(string userName, string password, Type expectedType)
        {
            var testLoginUser = new LoginUserDTO()
            {
                UserName = userName,
                Password = password
            };

            var result = await _sut.Login(testLoginUser);
            Assert.That(result.Result, Is.InstanceOf(expectedType));
        }

        [Test]
        [TestCase("ValidUsername", "ValidPassword1!", typeof(OkObjectResult), true)]
        [TestCase("ValidUsername", "ValidPassword1!", typeof(UnauthorizedObjectResult), false)]
        public async Task Login_GetsAuthorized_ReturnOk_Else_ReturnUnauthorized(string userName, string password, Type exepectedType, bool isAuthorized) 
        {
            _mockRepository.Setup(x => x.AuthorizeUserLogin(userName, password)).ReturnsAsync(isAuthorized);

            LoginUserDTO loginDto = new() { UserName = userName, Password = password };

            var result = await _sut.Login(loginDto);

            Assert.That(result.Result, Is.InstanceOf(exepectedType));
        }

        private static List<UserAccount> SeedUserAccounts(int count)
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
