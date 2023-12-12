
using AutoMapper;
using Entity;
using FinurligaFinanserWebAPI.Controllers;
using FinurligaFinanserWebAPI.DtoModels.BankAccountDTOs;
using Infrastructure.Enums;
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
            _sut = new BankAccountController(_bankAccountRepositoryMock.Object, _mockLogger.Object, _mockMapper.Object);

        }

        [Test]
        public async Task CreateBankAccount_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var bankAccountDto = new PostBankAccountDTO { NameOfAccount = "TestAccount", UserAccountId = 1 };
            var bankAccount = new BankAccount(1, "TestAccount", 1);
            _mockMapper.Setup(x => x.Map<BankAccountDTO>(bankAccount)).Returns(new BankAccountDTO{  
                Id = default,
                AccountNumber = default,
                NameOfAccount = "",
                Balance = default,
                Transactions = null,
            });

            _bankAccountRepositoryMock.Setup(x => x.CreateBankAccount(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((bankAccount, BankAccountValidationStatus.Valid));

            // Act
            var result = await _sut.CreateBankAccount(bankAccountDto);

            // Assert
            var createdAtResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtResult);
            Assert.That(createdAtResult.ActionName, Is.EqualTo("GetBankAccount"));
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
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        [TestCase("a")]
        [TestCase("aw")]
        [TestCase("")]
        public async Task CreateBankAccount_WhenCalled_ReturnsBadRequest_BankAccountName(string bankAccountName)
        {
            // Arrange
            var bankAccountDto = new PostBankAccountDTO { NameOfAccount = bankAccountName, UserAccountId = 1 };
            _bankAccountRepositoryMock.Setup(x => x.CreateBankAccount(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((new BankAccount(
                    1, bankAccountDto.NameOfAccount, bankAccountDto.UserAccountId
                    ), BankAccountValidationStatus.Invalid_BankAccountName));

            // Act
            var result = await _sut.CreateBankAccount(bankAccountDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(400));
            Assert.That(objectResult.Value, Is.EqualTo("Bank account name is invalid."));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public async Task CreateBankAccount_WhenCalled_ReturnsBadRequest_UserAccountId(int userAccountId)
        {
            // Arrange
            var bankAccountDto = new PostBankAccountDTO { NameOfAccount = "TestAccount", UserAccountId = userAccountId };
            _bankAccountRepositoryMock.Setup(x => x.CreateBankAccount(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((new BankAccount(1, bankAccountDto.NameOfAccount, bankAccountDto.UserAccountId), BankAccountValidationStatus.Invalid_UserAccountId));

            // Act
            var result = await _sut.CreateBankAccount(bankAccountDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(400));
            Assert.That(objectResult.Value, Is.EqualTo("User id is invalid."));
        }

        [Test]
        [TestCase(4)]
        public async Task CreateBankAccount_WhenCalled_ReturnsNotFound_UserAccountId(int userAccountId)
        {
            // Arrange
            var bankAccountDto = new PostBankAccountDTO { NameOfAccount = "TestAccount", UserAccountId = userAccountId };
            _bankAccountRepositoryMock.Setup(x => x.CreateBankAccount(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((new BankAccount(1, bankAccountDto.NameOfAccount, bankAccountDto.UserAccountId), BankAccountValidationStatus.NotFound));

            // Act
            var result = await _sut.CreateBankAccount(bankAccountDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(404));
            Assert.That(objectResult.Value, Is.EqualTo("User account not found."));
        }


        [Test]
        public async Task GetBankAcount_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var bankAccount = new BankAccount(1, "TestAccount", 1);
            _bankAccountRepositoryMock.Setup(x => x.GetBankAccount(It.IsAny<int>())).ReturnsAsync(bankAccount);
            _mockMapper.Setup(x => x.Map<BankAccountDTO>(bankAccount)).Returns(new BankAccountDTO());

            // Act
            var result = await _sut.GetBankAccount(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var objectResult = result.Result as OkObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(200));
            Assert.That(objectResult.Value, Is.InstanceOf<BankAccountDTO>());
        }

        [Test]
        public async Task GetBankAcount_WhenCalled_ReturnsNotFound()
        {
            // Arrange
            _bankAccountRepositoryMock.Setup(x => x.GetBankAccount(It.IsAny<int>())).ReturnsAsync((BankAccount)null!);

            // Act
            var result = await _sut.GetBankAccount(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var objectResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.Value, Is.EqualTo("Bank account not found."));
            Assert.That(objectResult.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetBankAcount_WhenCalled_Returns500()
        {
            // Arrange
            _bankAccountRepositoryMock.Setup(x => x.GetBankAccount(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetBankAccount(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.EqualTo("Internal Server Error"));
        }

        [Test]
        public async Task GetAllBankAcounts_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var bankAccounts = new List<BankAccount>
            {
                new BankAccount(1, "TestAccount", 1),
                new BankAccount(2, "TestAccount2", 1)
            };
            _bankAccountRepositoryMock.Setup(x => x.GetAllBankAccounts(It.IsAny<int>())).ReturnsAsync(bankAccounts);
            _mockMapper.Setup(x => x.Map<IEnumerable<BankAccountDTO>>(bankAccounts)).Returns(new List<BankAccountDTO>());

            // Act
            var result = await _sut.GetAllBankAccounts(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var objectResult = result.Result as OkObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(200));
            Assert.That(objectResult.Value, Is.InstanceOf<IEnumerable<BankAccountDTO>>());
        }

        [Test]
        public async Task GetAllBankAcounts_WhenCalled_ReturnsNotFound()
        {
            // Arrange
            _bankAccountRepositoryMock.Setup(x => x.GetAllBankAccounts(It.IsAny<int>())).ReturnsAsync((List<BankAccount>)null!);

            // Act
            var result = await _sut.GetAllBankAccounts(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var objectResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.Value, Is.EqualTo("Bank accounts not found."));
            Assert.That(objectResult.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetAllBankAcounts_WhenCalled_Returns500()
        {
            // Arrange
            _bankAccountRepositoryMock.Setup(x => x.GetAllBankAccounts(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAllBankAccounts(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.EqualTo("Internal Server Error"));
        }

        [Test]
        public async Task DeleteBankAccount_WhenCalled_ReturnsNotFound()
        {
            // Arrange
            _bankAccountRepositoryMock.Setup(x => x.GetBankAccount(It.IsAny<int>())).ReturnsAsync((BankAccount)null!);

            // Act
            var result = await _sut.DeleteBankAccount(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var objectResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.Value, Is.EqualTo("Bank account not found."));
            Assert.That(objectResult.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task DeleteBankAccount_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var bankAccount = new BankAccount(1, "TestAccount", 1);
            _bankAccountRepositoryMock.Setup(x => x.GetBankAccount(It.IsAny<int>())).ReturnsAsync(bankAccount);
            _bankAccountRepositoryMock.Setup(x => x.DeleteBankAccount(It.IsAny<int>())).ReturnsAsync((bankAccount, BankAccountValidationStatus.Valid));
            _mockMapper.Setup(x => x.Map<BankAccountDTO>(bankAccount)).Returns(new BankAccountDTO());

            // Act
            var result = await _sut.DeleteBankAccount(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var objectResult = result.Result as OkObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(200));
            Assert.That(objectResult.Value, Is.InstanceOf<BankAccountDTO>());
        }

        [Test]
        public async Task DeleteBankAccount_WhenCalled_Returns500()
        {
            // Arrange
            _bankAccountRepositoryMock.Setup(x => x.GetBankAccount(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _sut.DeleteBankAccount(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.EqualTo("Internal Server Error"));
        }

        [Test]
        public async Task DeleteBankAccount_WhenCalled_ReturnsBadRequest()
        {
            // Arrange
            var bankAccount = new BankAccount(1, "TestAccount", 1);
            _bankAccountRepositoryMock.Setup(x => x.GetBankAccount(It.IsAny<int>())).ReturnsAsync(bankAccount);
            _bankAccountRepositoryMock.Setup(x => x.DeleteBankAccount(It.IsAny<int>())).ReturnsAsync((bankAccount, BankAccountValidationStatus.ServerError));

            // Act
            var result = await _sut.DeleteBankAccount(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var objectResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(400));
            Assert.That(objectResult.Value, Is.EqualTo("Server error when deleting bankaccount"));

        }

        [Test]
        public async Task DeleteBankAccount_WhenCalled_ReturnsBadRequest_InvalidAmount()
        {
            // Arrange
            var bankAccount = new BankAccount(1, "TestAccount", 1);
            _bankAccountRepositoryMock.Setup(x => x.GetBankAccount(It.IsAny<int>())).ReturnsAsync(bankAccount);
            _bankAccountRepositoryMock.Setup(x => x.DeleteBankAccount(It.IsAny<int>())).ReturnsAsync((bankAccount, BankAccountValidationStatus.Invalid_amount));

            // Act
            var result = await _sut.DeleteBankAccount(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var objectResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(400));
            Assert.That(objectResult.Value, Is.EqualTo("Bank account needs to be empty"));
        }

        [Test]
        public async Task DeleteBankAccount_WhenCalled_ReturnsNotFound_BankAccountNotFound()
        {
            // Arrange
            var bankAccount = new BankAccount(1, "TestAccount", 1);
            _bankAccountRepositoryMock.Setup(x => x.GetBankAccount(It.IsAny<int>())).ReturnsAsync(bankAccount);
            _bankAccountRepositoryMock.Setup(x => x.DeleteBankAccount(It.IsAny<int>())).ReturnsAsync((bankAccount, BankAccountValidationStatus.NotFound));

            // Act
            var result = await _sut.DeleteBankAccount(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var objectResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(404));
            Assert.That(objectResult.Value, Is.EqualTo("Bank account not found."));
        }

    }
}
