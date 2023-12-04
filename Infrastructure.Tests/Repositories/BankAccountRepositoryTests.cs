
using Entity;
using Infrastructure.Enums;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class BankAccountRepositoryTests
    {
        private Mock<DataContext> _mockDataContext;

        private BankAccountRepository _sut;

        [SetUp]
        public void Setup()
        {
            _mockDataContext = new Mock<DataContext>();
            _sut = new BankAccountRepository(_mockDataContext.Object);
        }

        [Test]
        public async Task CreateBankAccount_WhenCalled_ReturnsBankAccountWithBankAccountName()
        {
            // Arrange
            var bankAccountName = "TestAccount";
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
    }
}
