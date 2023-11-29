using Entity;
using Infrastructure.Repositories;
using Infrastructure.Helpers;
using Infrastructure.Enums;
using Moq;
using Moq.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using static Infrastructure.Repositories.UserAccountRepository;

namespace Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class CreateUserAccountRepositoryTests
    {
        private Mock<DataContext> _mockDataContext;
        private Mock<ILogger<UserAccountRepository>> _mockLogger;
        private UserAccountRepository _sut;
        private UserAccount _userAccount;

        [SetUp]
        public void Setup()
        {
            _mockDataContext = new Mock<DataContext>(new DbContextOptions<DataContext>());
            _mockLogger = new Mock<ILogger<UserAccountRepository>>();
            _sut = new UserAccountRepository(_mockDataContext.Object, _mockLogger.Object);
            _userAccount = new UserAccount("Testuser", "Test", "User", Array.Empty<byte>(), "hashedPassword");
            _mockDataContext.Setup(m => m.UserAccounts.Add(_userAccount));
        }       

        //[Test]
        //public async Task CreateUserAccount_ShouldAddUserAccount_WhenUserIsValid()
        //{
        //    // Arrange
        //    _mockDataContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

        //    // Act
        //    var result = await _sut.CreateUserAccount(_userAccount);

        //    // Assert
        //    Assert.That(result, Is.EqualTo(_userAccount));
        //}

        //[Test]

        //public async Task CreateUserAccount_ShouldThrowUserNameAlreadyExistsException_WhenUserNameIsDuplicate()
        //{
        //    //Arrange
        //    var newUserAccount = new UserAccount("Testuser", "Arne1", "Karlsson", Array.Empty<byte>(), "hashedPassword");
        //    await _sut.CreateUserAccount(newUserAccount);
        //    var newUserAccount2 = new UserAccount("Testuser", "Nisse2", "Karlsson", Array.Empty<byte>(), "hashedPassword");
        //    await _sut.CreateUserAccount(newUserAccount2);

        //    var dbUpdateException = new DbUpdateException(
        //        "Duplicate username",
        //        new Microsoft.Data.SqlClient.SqlException(2627, null, null, null));

        //    _mockDataContext.Setup(m => m.SaveChangesAsync(default)).ThrowsAsync(dbUpdateException);

        //    // Act & Assert
        //    Assert.ThrowsAsync<UserNameAlreadyExistsException>(
        //        async () => await _sut.CreateUserAccount(newUserAccount2));
        //}        
    }
}
