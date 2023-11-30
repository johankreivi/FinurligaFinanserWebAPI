
using AutoMapper;
using Entity;
using FinurligaFinanserWebAPI.Controllers;
using FinurligaFinanserWebAPI.DtoModels.BankAccountDTOs;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinurligaFinanserWebAPI.Tests.Controllers
{
    [TestFixture]
    public class BankAccountControllerTests
    {
        private Mock<IBankAccountRepository> _bankAccountRepositoryMock;
        private Mock<ILogger<BankAccountController>> _mockLogger;
        private Mock<IMapper> _mockMapper;

        private BankAccountController _sut;

        [SetUp]
        public void Setup()
        {
            _bankAccountRepositoryMock = new Mock<IBankAccountRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<BankAccountController>>();
            _sut = new BankAccountController( _bankAccountRepositoryMock.Object, _mockLogger.Object, _mockMapper.Object );

        }

        [Test]
        public async Task CreateBankAccount_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var bankAccountDto = new PostBankAccountDTO { NameOfAccount = "TestAccount", UserAccountId = 1 };
            var bankAccount = new BankAccount(1, "TestAccount", 1);
            _mockMapper.Setup(x => x.Map<BankAccountDTO>(bankAccount)).Returns(new BankAccountDTO());
            _bankAccountRepositoryMock.Setup(
                x => x.CreateBankAccount(
                    It.IsAny<string>(),
                    It.IsAny<int>()
                ))
                .ReturnsAsync(bankAccount);

            // Act
            var result = await _sut.CreateBankAccount(bankAccountDto);

            // Assert
            var createdAtResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtResult);
            Assert.AreEqual("GetBankAccount", createdAtResult.ActionName);
            Assert.IsInstanceOf<BankAccountDTO>(createdAtResult.Value);

        }

        [Test]
        public async Task CreateBankAccount_WhenCalled_Returns500()
        {
            // Arrange
            var bankAccountDto = new PostBankAccountDTO { NameOfAccount = "TestAccount", UserAccountId = 1 };
            _bankAccountRepositoryMock.Setup(x => x.CreateBankAccount(It.IsAny<string>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.CreateBankAccount(bankAccountDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        [TestCase("400BankAccountName"), ]
        [TestCase("400UserAccountId")]
        public async Task CreateBankAccount_WhenCalled_ReturnsBadRequest(string exceptionMessage)
        {
            // Arrange
            var bankAccountDto = new PostBankAccountDTO { NameOfAccount = "TestAccount", UserAccountId = 1 };
            _bankAccountRepositoryMock.Setup(x => x.CreateBankAccount(It.IsAny<string>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _sut.CreateBankAccount(bankAccountDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task GetBankAccount()
        {
            // Arrange
            var bankAccount = new BankAccount(1, "TestAccount", 1);
            _bankAccountRepositoryMock.Setup(x => x.GetBankAccount(It.IsAny<int>())).ReturnsAsync(bankAccount);
            _mockMapper.Setup(x => x.Map<BankAccountDTO>(bankAccount)).Returns(new BankAccountDTO());

            // Act
            var result = await _sut.GetBankAccount(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetBankAccount_WhenCalled_Returns500()
        {
            // Arrange
            _bankAccountRepositoryMock.Setup(x => x.GetBankAccount(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetBankAccount(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
