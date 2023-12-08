using AutoMapper;
using Entity;
using FinurligaFinanserWebAPI.Controllers;
using FinurligaFinanserWebAPI.DtoModels.TransactionDTOs;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinurligaFinanserWebAPI.Tests.Controllers
{
    [TestFixture]
    public class TransactionControlerTests
    {
        private Mock<ITransactionRepository> _transactionRepositoryMock;
        private Mock<ILogger<TransactionController>> _loggerMock;
        private Mock<IMapper> _mapperMock;
        private TransactionController _sut;

        [SetUp]
        public void SetUp()
        {
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _loggerMock = new Mock<ILogger<TransactionController>>();
            _mapperMock = new Mock<IMapper>();
            _sut = new TransactionController(_transactionRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task Deposit_WhenCalledWithValidDepositDTO_ReturnsOkObjectResult()
        {
            // Arrange
            var depositDTO = new DepositDTO
            {
                ReceivingAccountNumber = 123456789,
                Amount = 100
            };
            var transaction = new Transaction(123456789, null, 100, "deposit");
            _mapperMock.Setup(x => x.Map<DepositConfirmationDTO>(It.IsAny<Transaction>())).Returns(new DepositConfirmationDTO{ AccountBalance = 100, Amount = 100, ReceivingAccountNumber = 123456789, TimeStamp = new DateTime(), Type = TransactionType.Deposit });
            _transactionRepositoryMock.Setup(x => x.Deposit(It.IsAny<Transaction>())).ReturnsAsync(transaction);

            // Act
            var result = await _sut.Deposit(depositDTO);

            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.That(okResult, Is.TypeOf<OkObjectResult>());
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.TypeOf<DepositConfirmationDTO>());
            Assert.That(okResult.Value, Is.Not.Null);
        }

        [Test]
        public async Task PostTransaction_WhenCalledWithValidPostTransactionDTO_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var postTransactionDTO = new PostTransactionDTO
            {
                ReceivingAccountNumber = 123456789,
                SendingAccountNumber = 987654321,
                Amount = 100,
                Message = "test"
            };
            var transaction = new Transaction(123456789, 987654321, 100, "test");
            _mapperMock.Setup(x => x.Map<Transaction>(It.IsAny<PostTransactionDTO>())).Returns(transaction);
            _mapperMock.Setup(x => x.Map<TransactionConfirmationDTO>(It.IsAny<Transaction>())).Returns(new TransactionConfirmationDTO { AccountBalance = 100, Amount = 100, ReceivingAccountNumber = 123456789, SendingAccountNumber = 987654321, Type = TransactionType.Transaction, Message = "test", TimeStamp = new DateTime()});
            _transactionRepositoryMock.Setup(x => x.CreateTransaction(It.IsAny<Transaction>())).ReturnsAsync((transaction, Infrastructure.Enums.TransactionStatus.Success));

            // Act
            var result = await _sut.PostTransaction(postTransactionDTO);

            var createdAtActionResult = result.Result as CreatedAtActionResult;

            // Assert
            Assert.That(createdAtActionResult, Is.TypeOf<CreatedAtActionResult>());
            Assert.That(createdAtActionResult.StatusCode, Is.EqualTo(201));
            Assert.That(createdAtActionResult.Value, Is.TypeOf<TransactionConfirmationDTO>());
            Assert.That(createdAtActionResult.Value, Is.Not.Null);
        }

        [Test]
        [TestCase(0, "Error, trying to send an invalid amount.")]
        [TestCase(1000, "Error, trying to send an amount that does not exist on the sender's bankaccount. Insufficient Amount.")]
        public async Task PostTransaction_WhenCalledWithInvalidPostTransactionDTO_ReturnsBadRequestObjectResult(int amount, string expectedErrorMessage)
        {
            // Arrange
            var postTransactionDTO = new PostTransactionDTO
            {
                ReceivingAccountNumber = 123456789,
                SendingAccountNumber = 987654321,
                Amount = amount,
                Message = "test"
            };
            var transaction = new Transaction(123456789, 987654321, amount, "test");
            _mapperMock.Setup(x => x.Map<Transaction>(It.IsAny<PostTransactionDTO>())).Returns(transaction);
            if (amount == 0) _transactionRepositoryMock.Setup(x => x.CreateTransaction(It.IsAny<Transaction>())).ReturnsAsync((transaction, Infrastructure.Enums.TransactionStatus.Invalid_Amount));
            if (amount == 1000) _transactionRepositoryMock.Setup(x => x.CreateTransaction(It.IsAny<Transaction>())).ReturnsAsync((transaction, Infrastructure.Enums.TransactionStatus.Insufficient_Funds));

            // Act
            var result = await _sut.PostTransaction(postTransactionDTO);

            var badRequestObjectResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.That(badRequestObjectResult, Is.TypeOf<BadRequestObjectResult>());
            Assert.That(badRequestObjectResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestObjectResult.Value, Is.TypeOf<string>());
            Assert.That(badRequestObjectResult.Value, Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public async Task PostTransaction_WhenCalledWithValidPostTransactionDTOAndTransactionStatusIsNotFound_ReturnsNotFoundObjectResult()
        {
            // Arrange
            var postTransactionDTO = new PostTransactionDTO
            {
                ReceivingAccountNumber = 123456789,
                SendingAccountNumber = 987654321,
                Amount = 100,
                Message = "test"
            };
            var transaction = new Transaction(123456789, 987654321, 100, "test");
            _mapperMock.Setup(x => x.Map<Transaction>(It.IsAny<PostTransactionDTO>())).Returns(transaction);
            _transactionRepositoryMock.Setup(x => x.CreateTransaction(It.IsAny<Transaction>())).ReturnsAsync((transaction, Infrastructure.Enums.TransactionStatus.BankAccount_Not_Found));

            // Act
            var result = await _sut.PostTransaction(postTransactionDTO);

            var notFoundObjectResult = result.Result as NotFoundObjectResult;

            // Assert
            Assert.That(notFoundObjectResult, Is.TypeOf<NotFoundObjectResult>());
            Assert.That(notFoundObjectResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundObjectResult.Value, Is.TypeOf<string>());
            Assert.That(notFoundObjectResult.Value, Is.EqualTo("Error, BankAccount Not Found."));
        }

        [Test]
        public async Task PostTransaction_WhenCalled_returns500()
        {
            // Arrange
            var postTransactionDTO = new PostTransactionDTO
            {
                ReceivingAccountNumber = 123456789,
                SendingAccountNumber = 987654321,
                Amount = 100,
                Message = "test"
            };

            _transactionRepositoryMock.Setup(x => x.CreateTransaction(It.IsAny<Transaction>())).ThrowsAsync(new Exception());

            // Act
            var result = await _sut.PostTransaction(postTransactionDTO);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetTransaction_WhenCalledWithValidId_ReturnsOkObjectResult()
        {
            // Arrange
            var transaction = new Transaction(123456789, 987654321, 100, "test");
            _mapperMock.Setup(x => x.Map<TransactionConfirmationDTO>(It.IsAny<Transaction>())).Returns(new TransactionConfirmationDTO { AccountBalance = 100, Amount = 100, ReceivingAccountNumber = 123456789, SendingAccountNumber = 987654321 });
            _transactionRepositoryMock.Setup(x => x.GetTransaction(It.IsAny<int>())).ReturnsAsync(transaction);

            // Act
            var result = await _sut.GetTransaction(1);

            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.That(okResult, Is.TypeOf<OkObjectResult>());
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.TypeOf<TransactionConfirmationDTO>());
            Assert.That(okResult.Value, Is.Not.Null);
        }

        [Test]
        public async Task GetTransaction_WhenCalledWithInvalidId_ReturnsNotFoundObjectResult()
        {
            // Arrange
            _transactionRepositoryMock.Setup(x => x.GetTransaction(It.IsAny<int>())).ReturnsAsync((Transaction)null!);

            // Act
            var result = await _sut.GetTransaction(1);

            var notFoundObjectResult = result.Result as NotFoundObjectResult;

            // Assert
            Assert.That(notFoundObjectResult, Is.TypeOf<NotFoundObjectResult>());
            Assert.That(notFoundObjectResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundObjectResult.Value, Is.TypeOf<string>());
            Assert.That(notFoundObjectResult.Value, Is.EqualTo("Transaction not found"));
        }

        [Test]
        public async Task GetTransaction_WhenCalled_returns500()
        {
            // Arrange
            _transactionRepositoryMock.Setup(x => x.GetTransaction(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetTransaction(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetTransactionsByBankAccountId_WhenCalledWithValidId_ReturnsOkObjectResult()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction(123456789, 987654321, 100, "test"),
                new Transaction(123456789, 987654321, 100, "test")
            };
            _mapperMock.Setup(x => x.Map<IEnumerable<TransactionConfirmationDTO>>(It.IsAny<IEnumerable<Transaction>>())).Returns(new List<TransactionConfirmationDTO> { new TransactionConfirmationDTO { AccountBalance = 100, Amount = 100, ReceivingAccountNumber = 123456789, SendingAccountNumber = 987654321 }, new TransactionConfirmationDTO { AccountBalance = 100, Amount = 100, ReceivingAccountNumber = 123456789, SendingAccountNumber = 987654321 } });
            _transactionRepositoryMock.Setup(x => x.GetTransactionsByBankAccountId(It.IsAny<int>())).ReturnsAsync(transactions);

            // Act
            var result = await _sut.GetTransactionsByBankAccountId(1);

            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.That(okResult, Is.TypeOf<OkObjectResult>());
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.TypeOf<List<TransactionConfirmationDTO>>());
            Assert.That(okResult.Value, Is.Not.Null);
        }

        [Test]
        public async Task GetTransactionsByBankAccountId_WhenCalledWithInvalidId_ReturnsNotFoundObjectResult()
        {
            // Arrange
            _transactionRepositoryMock.Setup(x => x.GetTransactionsByBankAccountId(It.IsAny<int>())).ReturnsAsync((List<Transaction>)null!);

            // Act
            var result = await _sut.GetTransactionsByBankAccountId(1);

            var notFoundObjectResult = result.Result as NotFoundObjectResult;

            // Assert
            Assert.That(notFoundObjectResult, Is.TypeOf<NotFoundObjectResult>());
            Assert.That(notFoundObjectResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundObjectResult.Value, Is.TypeOf<string>());
            Assert.That(notFoundObjectResult.Value, Is.EqualTo("Transactions not found"));
        }

        [Test]
        public async Task GetTransactionsByBankAccountId_WhenCalled_returns500()
        {
            // Arrange
            _transactionRepositoryMock.Setup(x => x.GetTransactionsByBankAccountId(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetTransactionsByBankAccountId(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
