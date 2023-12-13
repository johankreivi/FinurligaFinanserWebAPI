
using Entity;
using Infrastructure.Enums;
using Infrastructure.Helpers;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class BankAccountRepositoryTests
    {

        private Mock<DataContext> _mockDataContext;
        private BankAccountRepository _sut;
        private Mock<UserAccountRepository> _mockUserAccountRepository;
        private Mock<ILogger<UserAccountRepository>> _logger;
        private List<UserAccount> _userAccounts;
        private List<BankAccount> _bankAccounts;

        [SetUp]
        public void Setup()
        {
            _mockDataContext = new Mock<DataContext>();

            _logger = new Mock<ILogger<UserAccountRepository>>();
            _userAccounts = SeedUserAccounts();
            _bankAccounts = SeedBankAccounts();
            _mockDataContext.Setup(x => x.UserAccounts).ReturnsDbSet(_userAccounts);
            _mockDataContext.Setup(x => x.BankAccounts).ReturnsDbSet(_bankAccounts);
            _mockUserAccountRepository = new Mock<UserAccountRepository>(_mockDataContext.Object);
            _sut = new BankAccountRepository(_mockDataContext.Object);

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
                bankAccounts.Add(new BankAccount(i, $"TestAccount{i}", i)
                { Id = i});
            }

            return bankAccounts;
        }

        [Test]
        public async Task CreateBankAccount_WhenCalled_ReturnsBankAccountWithBankAccountName()
        {
            // Arrange
            var bankAccountName = "TestAccount1";
            var userAccountId = 1;

            // Act
            var (resultBankAccount, resultValidationStatus) = await _sut.CreateBankAccount(bankAccountName, userAccountId);

            // Assert
            Assert.That(resultBankAccount, Has.Property("NameOfAccount").EqualTo(bankAccountName));
            Assert.That(resultValidationStatus, Is.EqualTo(BankAccountValidationStatus.Valid));
            Assert.That(resultBankAccount, Is.TypeOf<BankAccount>());
        }

        [Test]
        public async Task CreateBankAccount_WhenCalled_ReturnsBankAccountWithBankAccountNumber()
        {
            // Arrange
            var bankAccountName = "TestAccount";
            var userAccountId = 1;

            // Act
            var (resultBankAccount, resultValidationStatus) = await _sut.CreateBankAccount(bankAccountName, userAccountId);

            // Assert
            Assert.That(resultBankAccount, Has.Property("AccountNumber").GreaterThan(0));
            Assert.That(resultValidationStatus, Is.EqualTo(BankAccountValidationStatus.Valid));
            Assert.That(resultBankAccount, Is.TypeOf<BankAccount>());
        }

        
        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("aw")]
        [TestCase("    ")]
        [TestCase("ab  d")]
        [TestCase(" dff")]
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaasfsefsefsef")]
        public async Task CreateBankAccount_WhenBankAccountNameIsInvalid_ReturnsInvalidBankAccountName(string bankAccountName)
        {
            // Arrange
            var userAccountId = 1;

            // Act
            var (resultBankAccount, resultValidationStatus) = await _sut.CreateBankAccount(bankAccountName, userAccountId);

            // Assert
            Assert.That(resultBankAccount, Is.Null);
            Assert.That(resultValidationStatus, Is.EqualTo(BankAccountValidationStatus.Invalid_BankAccountName));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task CreateBankAccount_WhenUserAccountIdIsInvalid_ReturnsInvalidUserAccountId(int userAccountId)
        {
            // Arrange
            var bankAccountName = "TestAccount";

            // Act
            var (resultBankAccount, resultValidationStatus) = await _sut.CreateBankAccount(bankAccountName, userAccountId);

            // Assert
            Assert.That(resultBankAccount, Is.Null);
            Assert.That(resultValidationStatus, Is.EqualTo(BankAccountValidationStatus.Invalid_UserAccountId));
        }

        [Test]
        public async Task CreateBankAccount_WhenUserAccountIdDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var bankAccountName = "TestAccount";
            var userAccountId = 10;

            // Act
            var (resultBankAccount, resultValidationStatus) = await _sut.CreateBankAccount(bankAccountName, userAccountId);

            // Assert
            Assert.That(resultBankAccount, Is.Null);
            Assert.That(resultValidationStatus, Is.EqualTo(BankAccountValidationStatus.NotFound));
        }

        [Test]
        public async Task GetAllBankAccounts_WhenCalled_ReturnsListOfBankAccounts()
        {
            // Arrange
            var userAccountId = 1;

            // Act
            var result = await _sut.GetAllBankAccounts(userAccountId);

            // Assert
            Assert.That(result, Is.TypeOf<List<BankAccount>>());
            Assert.That(result.ToList().Count() == 1);
        }

        [Test]
        [TestCase(6)]
        public async Task GetAllBankAccounts_WhenCalled_ReturnsNull(int userAccountId)
        {
            // Arrange

            // Act
            var result = await _sut.GetAllBankAccounts(userAccountId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        [TestCase(1)]
        public async Task GetBankAccount_WhenCalled_ReturnsBankAccount(int id)
        {
            // Arrange
            _mockDataContext.Setup(x => x.BankAccounts.FindAsync(id)).ReturnsAsync(_bankAccounts.Find(x => x.Id == id));

            // Act
            var result = await _sut.GetBankAccount(id);

            // Assert
            Assert.That(result, Is.TypeOf<BankAccount>());
            Assert.That(result, Has.Property("Id").EqualTo(id));
            Assert.That(result, Has.Property("AccountNumber").EqualTo(1));
            Assert.That(result, Has.Property("NameOfAccount").EqualTo("TestAccount1"));
            Assert.That(result, Has.Property("UserAccountId").EqualTo(1));
            Assert.That(result, Has.Property("Transactions").Empty);
        }

        [Test]
        [TestCase(10)]
        public async Task GetBankAccount_WhenCalled_ReturnsNull(int id)
        {
            // Arrange

            // Act
            var result = await _sut.GetBankAccount(id);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteBankAccount_WhenSuccessful_ReturnsValidStatus()
        {


        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var _dataContext = new DataContext(options);

            var bankAccount = new BankAccount(1, "TestAccount1", 1);

            _dataContext.BankAccounts.Add(bankAccount);
            await _dataContext.SaveChangesAsync();

            var sut = new BankAccountRepository(_dataContext);

            // Act
            var (resultBankAccount, resultValidationStatus) = await sut.DeleteBankAccount(1);


            // Assert
            Assert.That(resultBankAccount, Is.TypeOf<BankAccount>());
            Assert.That(resultValidationStatus, Is.EqualTo(BankAccountValidationStatus.Valid));
        }

        [Test]
        public async Task DeleteBankAccount_WhenBankAccountDoesNotExist_ReturnsNotFoundStatus()
        {
            // Arrange
            var id = 10;

            // Act
            var (resultBankAccount, resultValidationStatus) = await _sut.DeleteBankAccount(id);

            // Assert
            Assert.That(resultBankAccount, Is.Null);
            Assert.That(resultValidationStatus, Is.EqualTo(BankAccountValidationStatus.NotFound));
        }

        [Test]
        public async Task DeleteBankAccount_WhenBankAccountHasBalance_ReturnsInvalidAmountStatus()
        {
            // Arrange
            var id = 1;
            var bankaccount = _bankAccounts.Find(x => x.Id == id);
            bankaccount.Balance = 100;

            _mockDataContext.Setup(x => x.BankAccounts.FindAsync(id)).ReturnsAsync(bankaccount);


            // Act
            var (resultBankAccount, resultValidationStatus) = await _sut.DeleteBankAccount(id);

            // Assert
            Assert.That(resultBankAccount, Is.Null);
            Assert.That(resultValidationStatus, Is.EqualTo(BankAccountValidationStatus.Invalid_amount));
        }
    }
}
