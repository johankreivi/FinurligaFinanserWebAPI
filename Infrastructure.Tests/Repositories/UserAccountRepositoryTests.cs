using Entity;
using Infrastructure.Repositories;
using Infrastructure.Helpers;
using Infrastructure.Enums;
using Moq;
using Moq.EntityFrameworkCore;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class UserAccountRepositoryTests
    {
        private Mock<DataContext> _mockDataContext;
        private List<UserAccount> _userAccounts;
        private Mock<ILogger<UserAccountRepository>> _logger;

        private UserAccountRepository _sut;

        [SetUp]
        public void Setup()
        {
            _userAccounts = SeedUserAccounts();
            _logger = new Mock<ILogger<UserAccountRepository>>();
            _mockDataContext = new Mock<DataContext>();
            _mockDataContext.Setup(x => x.UserAccounts).ReturnsDbSet(_userAccounts);

            _sut = new UserAccountRepository(_mockDataContext.Object, _logger.Object);            
        }

        [Test]
        public void TestUserAccountCreation()
        {
            var _userAccount = new UserAccount("testuser", "Test", "User", Array.Empty<byte>(), "");

            Assert.Multiple(() =>
            {
                // Verifiera att UserAccount-objektet har de förväntade värdena.
                Assert.That(_userAccount.UserName, Is.EqualTo("testuser"));
                Assert.That(_userAccount.FirstName, Is.EqualTo("Test"));
                Assert.That(_userAccount.LastName, Is.EqualTo("User"));                
            });
            Assert.Multiple(() =>
            {
                Assert.That(_userAccount.PasswordSalt, Is.Not.Null);
                Assert.That(_userAccount.PasswordHash, Is.Not.Null);
            });
        }

        [Test]
        [TestCase("aaaaaaa", UserValidationStatus.NotValid_Password_Does_Not_Meet_Requirements)] // För kort
        [TestCase("aaaaaaaa", UserValidationStatus.NotValid_Password_Does_Not_Meet_Requirements)] // Tillräckligt långt, liten bokstav, men ingen stor bokstav, siffra eller specialtecken.
        [TestCase("aaaaaaaB", UserValidationStatus.NotValid_Password_Does_Not_Meet_Requirements)] // Tillräckligt långt, liten bokstav, stor bokstav, men ingen siffra eller specialtecken.
        [TestCase("aaaaaaB1", UserValidationStatus.NotValid_Password_Does_Not_Meet_Requirements)] // Tillräckligt långt, liten bokstav, stor bokstav och siffra, men inget specialtecken.        
        [TestCase("AAAAAA1?", UserValidationStatus.NotValid_Password_Does_Not_Meet_Requirements)] // Tillräckligt långt, stor bokstav, siffra och specialtecken, men ingen liten bokstav.
        [TestCase("AAAAAb1?", UserValidationStatus.Valid)] // Uppfyller alla krav

        public async Task TestPasswordIsValid(string password, UserValidationStatus expectedValidationStatus)
        {
            string validUserName = "validUName";
            string validFirstName = "validFName";
            string validLastName = "validLName";
            var result = await _sut.CreateUserAccount(validUserName, validFirstName, validLastName, password);

            Assert.That(result.Item2, Is.EqualTo(expectedValidationStatus));
        }

        [Test]
        public void TestPasswordHashing()
        {
            // Arrange
            var plainPassword = "TestPassword123!";
            var salt = PasswordHasher.GenerateSalt();
            var hashedPassword = PasswordHasher.HashPassword(plainPassword, salt);

            // Act
            var rehashedPassword = PasswordHasher.HashPassword(plainPassword, salt);

            // Assert
            Assert.That(hashedPassword, Is.Not.EqualTo(plainPassword), "Hashed password should not be the same as plain password.");
            Assert.That(hashedPassword, Is.EqualTo(rehashedPassword), "Rehashed password should match the original hash.");
        }

        [Test]
        public void TestPasswordSaltCreation()
        {
            // Arrange
            var userAccount1 = new UserAccount("testuser1", "Test", "User", PasswordHasher.GenerateSalt(), "");
            var userAccount2 = new UserAccount("testuser2", "Test", "User", PasswordHasher.GenerateSalt(), "");

            // Act
            var salt1 = userAccount1.PasswordSalt;
            var salt2 = userAccount2.PasswordSalt;

            // Assert
            Assert.That(salt1, Is.Not.EqualTo(salt2), "Salts for different user accounts should be unique.");
        }

        [Test]
        [TestCase(null, UserValidationStatus.NotValid_UserName_Is_NullOrEmpty)] // null användarnamn bör inte vara giltigt
        [TestCase("", UserValidationStatus.NotValid_UserName_Is_NullOrEmpty)] // tomt användarnamn bör inte vara giltigt
        [TestCase("aaaaa", UserValidationStatus.NotValid_UserNameLength_Too_Short)] // för kort användarnamn (mindre än 6 tecken)
        [TestCase("abbbbb", UserValidationStatus.Valid)] // giltigt användarnamn (minst 6 tecken)        

        public async Task TestValidUserName(string userName, UserValidationStatus expectedValidationStatus)
        {
            string validFirstName = "validFName";
            string validLastName = "validLName";
            string validPassword = "AAAAAb1?";
            var result = await _sut.CreateUserAccount(userName, validFirstName, validLastName, validPassword);

            Assert.That(result.Item2, Is.EqualTo(expectedValidationStatus));
        }

        [Test]
        [TestCase(null, UserValidationStatus.NotValid_Name_Is_NullOrEmpty)] // null firstName ej giltigt
        [TestCase("", UserValidationStatus.NotValid_Name_Is_NullOrEmpty)] // tomt firstName ej giltigt
        [TestCase("a", UserValidationStatus.NotValid_NameLength_Too_Short)] // kortare firstName ej giltigt
        [TestCase("aa", UserValidationStatus.Valid)] // tillräckligt långt firstName giltigt
        [TestCase("åke", UserValidationStatus.Valid)] // innehåller giltig specialbokstav, firstName giltigt
        [TestCase("a a", UserValidationStatus.NotValid_Name_Contains_Invalid_Characters)] // förnamnet innehåller ett mellanslag, ej giltigt
        [TestCase("a8a", UserValidationStatus.NotValid_Name_Contains_Invalid_Characters)] // förnamnet innehåller en siffra, ej giltigt
        [TestCase("a!a", UserValidationStatus.NotValid_Name_Contains_Invalid_Characters)] // förnamnet innehåller ett specialtecken, ej giltigt

        public async Task TestValidFirstName(string firstName, UserValidationStatus expectedValidationStatus)
        {
            string validUserName = "validUName";
            string validLastName = "validLName";
            string validPassword = "AAAAAb1?";

            var result = await _sut.CreateUserAccount(validUserName, firstName, validLastName, validPassword);

            Assert.That(result.Item2, Is.EqualTo(expectedValidationStatus));
        }

        [Test]
        [TestCase(null, UserValidationStatus.NotValid_Name_Is_NullOrEmpty)] // null lastName, ej giltigt
        [TestCase("", UserValidationStatus.NotValid_Name_Is_NullOrEmpty)] // tomt lastName, ej giltigt
        [TestCase("a", UserValidationStatus.NotValid_NameLength_Too_Short)] // kortare lastName, ej giltigt
        [TestCase("aa", UserValidationStatus.Valid)] // tillräckligt långt lastName, giltigt
        [TestCase("örn", UserValidationStatus.Valid)] // innehåller giltig specialbokstav, lastName giltigt
        [TestCase("a a", UserValidationStatus.NotValid_Name_Contains_Invalid_Characters)] // efternamnet innehåller ett mellanslag, ej giltigt
        [TestCase("a8a", UserValidationStatus.NotValid_Name_Contains_Invalid_Characters)] // efternamnet innehåller en siffra, ej giltigt
        [TestCase("a!a", UserValidationStatus.NotValid_Name_Contains_Invalid_Characters)] // efternamnet innehåller ett specialtecken, ej giltigt

        public async Task TestValidLastName(string lastName, UserValidationStatus expectedValidationStatus)
        {
            string validUserName = "validUName";
            string validFirstName = "validFName";
            string validPassword = "AAAAAb1?";
            var result = await _sut.CreateUserAccount(validUserName, validFirstName, lastName, validPassword); // TODO: Should test RegisterUser but then Moq is required

            Assert.That(result.Item2, Is.EqualTo(expectedValidationStatus));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(6)]
        public async Task GetAllUserAccountsAsync_ShouldReturnListWithCorrectAmountOfUsers(int take)
        {
            int expectedCount = take > _userAccounts.Count ? _userAccounts.Count : 
                                take < 0 ? 0 :take;

            var useraccounts = await _sut.GetAllUserAccountsAsync(take);

            Assert.That(useraccounts, Has.Count.EqualTo(expectedCount));
        }

        [Test]
        [TestCase(-1, true)]
        [TestCase(0, false)]
        public async Task GetOneUser_IfUserExist_ReturnUser_Else_ReturnNull(int id, bool isNullExpected)
        {
            _mockDataContext.Setup(x => x.UserAccounts.FindAsync(id)).ReturnsAsync(_userAccounts.Find(x => x.Id == id));
            
            var result = await _sut.GetOneUser(id);

            Assert.That(result is null, Is.EqualTo(isNullExpected));
            if(!isNullExpected)
            {
                Assert.That(result, Is.InstanceOf(typeof(UserAccount)));
            }
        }

        [Test]
        [TestCase("notExistingUsername", "ValidPassword1!")]
        public async Task AuthorizeUserLogin_UserDoesNotExist_ReturnFalse(string userName, string password)
        {
            var result = await _sut.AuthorizeUserLogin(userName, password);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("UserAccount0", "notMatchingPassword")]
        public async Task AuthorizeUserLogin_UserExistButPasswordDoesNotMatch_ReturnFalse(string userName, string password)
        {
            var result = await _sut.AuthorizeUserLogin(userName, password);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("UserAccount0", "Password0!")]
        public async Task AuthorizeUserLogin_UserExistAndPasswordMatch_ReturnTrue(string userName, string password)
        {
            var result = await _sut.AuthorizeUserLogin(userName, password);

            Assert.That(result, Is.True);
        }

        private static List<UserAccount> SeedUserAccounts()
        {
            var userAccounts = new List<UserAccount>();

            for (int i = 0; i < 5; i++)
            {
                var salt = PasswordHasher.GenerateSalt();
                var hash = PasswordHasher.HashPassword($"Password{i}!", salt);
                userAccounts.Add(new UserAccount($"UserAccount{i}", $"FirstName{i}", $"LastName{i}", salt, hash) 
                            { Id = i});

            }

            return userAccounts;
        }
    }
}