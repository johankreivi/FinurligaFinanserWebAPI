using Moq;
using Entity;
using Infrastructure.Helpers;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using Infrastructure.Enums;

namespace Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class TransactionRepositoryTests
    {
        private Mock<DataContext> _mockDataContext;
        private TransactionRepository _sut;
        private List<UserAccount> _userAccounts;
        private List<BankAccount> _bankAccounts;
        private List<Transaction> _transactions;

        [SetUp]
        public void Setup()
        {
            _mockDataContext = new Mock<DataContext>();
            _userAccounts = SeedUserAccounts();
            _bankAccounts = SeedBankAccounts();
            _transactions = SeedTransactions();
            _mockDataContext.Setup(x => x.UserAccounts).ReturnsDbSet(_userAccounts);
            _mockDataContext.Setup(x => x.BankAccounts).ReturnsDbSet(_bankAccounts);
            _mockDataContext.Setup(x => x.Transactions).ReturnsDbSet(_transactions);
            _sut = new TransactionRepository(_mockDataContext.Object);
        }

        private static List<UserAccount> SeedUserAccounts()
        {
            var userAccounts = new List<UserAccount>();

            for (int i = 1; i < 7; i++)
            {
                var salt = PasswordHasher.GenerateSalt();
                var hash = PasswordHasher.HashPassword($"Password{i}!", salt);
                userAccounts.Add(new UserAccount($"TestAccount{i}", $"FirstName{i}", $"LastName{i}", salt, hash)
                { Id = i });

            }

            return userAccounts;
        }

        private static List<BankAccount> SeedBankAccounts()
        {
            var bankAccounts = new List<BankAccount>();

            for (int i = 1; i < 6; i++)
            {
                bankAccounts.Add(new BankAccount(i, $"TestAccount{i}", i, i)
                { Id = i });

            }

            return bankAccounts;
        }

        private static List<Transaction> SeedTransactions()
        {
            var transactions = new List<Transaction>();

            for (int i = 1; i < 6; i++)
            {
                var transaction = new Transaction(i, i + 1, i, $"TestMessage{i}")
                {
                    BankAccountId = i
                };
                transactions.Add(transaction);
            }

            return transactions;
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public async Task CreateTransaction_WhenCalledWithInvalidAmount_ReturnsNullAndTransactionStatusInvalidAmount(decimal amount)
        {
            var transaction = new Transaction(1, 2, amount, "TestMessage");

            var (result, status) = await _sut.CreateTransaction(transaction);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(status, Is.EqualTo(TransactionStatus.Invalid_Amount));
            });
        }

        [Test]
        public async Task CreateTransaction_WhenCalledWithNonExistingSendingAccount_ReturnsNullAndTransactionStatusBankAccountNotFound()
        {
            var transaction = new Transaction(2, 0, 1, "TestMessage");

            var (result, status) = await _sut.CreateTransaction(transaction);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(status, Is.EqualTo(TransactionStatus.BankAccount_Not_Found));
            });
        }

        [Test]
        public async Task CreateTransaction_WhenCalledWithNonExistingReceivingAccount_ReturnsNullAndTransactionStatusBankAccountNotFound()
        {
            var transaction = new Transaction(0, 2, 1, "TestMessage");

            var (result, status) = await _sut.CreateTransaction(transaction);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(status, Is.EqualTo(TransactionStatus.BankAccount_Not_Found));
            });
        }

        [Test]
        public async Task CreateTransaction_WhenCalledWithInsufficientFunds_ReturnsNullAndTransactionStatusInsufficientFunds()
        {
            var transaction = new Transaction(1, 2, 100, "TestMessage");

            var (result, status) = await _sut.CreateTransaction(transaction);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(status, Is.EqualTo(TransactionStatus.Insufficient_Funds));
            });
        }

        [Test]
        public async Task CreateTransaction_WhenCalledWithValidTransaction_ReturnsTransactionAndTransactionStatusSuccess()
        {
            var transaction = new Transaction(4, 5, 1, "TestMessage");

            var (result, status) = await _sut.CreateTransaction(transaction);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf<Transaction>());
                Assert.That(status, Is.EqualTo(TransactionStatus.Success));
            });
        }

        [Test]
        public async Task GetTransaction_WhenCalledWithValidId_ReturnsTransaction()
        {
            _mockDataContext.Setup(_mockDataContext => _mockDataContext.Transactions.FindAsync(It.IsAny<int>()))
                .ReturnsAsync(new Transaction(1, 2, 1, "TestMessage"));

            var id = 1;

            var result = await _sut.GetTransaction(id);


            Assert.That(result, Is.TypeOf<Transaction>());
        }

        [Test]
        public async Task GetTransaction_WhenCalledWithInvalidId_ReturnsNull()
        {
            _mockDataContext.Setup(_mockDataContext => _mockDataContext.Transactions.FindAsync(It.IsAny<int>()))
    .ReturnsAsync((Transaction)null!);

            var id = 10;

            var result = await _sut.GetTransaction(id);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Deposit_WhenCalledWithNonExistingReceivingAccount_ReturnsNull()
        {
            var transaction = new Transaction(0, 2, 1, "TestMessage");

            var result = await _sut.Deposit(transaction);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Deposit_WhenCalledWithValidTransaction_ReturnsTransaction()
        {
            var transaction = new Transaction(4, 5, 1, "TestMessage");

            var result = await _sut.Deposit(transaction);

            Assert.That(result, Is.TypeOf<Transaction>());
        }

        [Test]
        public async Task GetTransactionsByBankAccountId_WhenCalledWithNonExistingBankAccountId_ReturnsNull()
        {
            var id = 10;

            var result = await _sut.GetTransactionsByBankAccountId(id);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetTransactionsByBankAccountId_WhenCalledWithValidBankAccountId_ReturnsTransactions()
        {
            //Arrange
            var id = 1;

            //Act
            var result = await _sut.GetTransactionsByBankAccountId(id);

            //Assert
            Assert.That(result, Is.TypeOf<List<Transaction>>());
            Assert.That(result.ToList(), Has.Count.EqualTo(1));
        }
    }
}