using Entity;
using Infrastructure.Repositories;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;


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
    }
}
