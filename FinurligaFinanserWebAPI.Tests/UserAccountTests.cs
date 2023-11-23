using Entity;

namespace FinurligaFinanserWebAPI.Tests
{
    [TestFixture]
    public class UserAccountTests
    {
        private UserAccount _userAccount;

        [SetUp]
        public void Setup()
        {            
            _userAccount = new UserAccount("testuser", "Test", "User", "TestPassword123!");
        }

        [Test]
        public void TestUserAccountCreation()
        {
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
        [TestCase("aaaaaaa", false)] // För kort
        [TestCase("aaaaaaaa", false)] // Tillräckligt långt, liten bokstav, men ingen stor bokstav, siffra eller specialtecken.
        [TestCase("aaaaaaaB", false)] // Tillräckligt långt, liten bokstav, stor bokstav, men ingen siffra eller specialtecken.
        [TestCase("aaaaaaB1", false)] // Tillräckligt långt, liten bokstav, stor bokstav och siffra, men inget specialtecken.        
        [TestCase("AAAAAA1?", false)] // Tillräckligt långt, stor bokstav, siffra och specialtecken, men ingen liten bokstav.
        
        public void TestPasswordIsValid(string password, bool isValid)
        {
            bool result = true;
            try
            {
                var userAccount = new UserAccount("userName", "Test", "User", password);
            }
            catch (Exception)
            {
                // En exception antyder att skapandet inte är giltigt enligt våra [Required]-attribut
                result = false;
            }

            // Assert
            Assert.That(result, Is.EqualTo(isValid));
        }

        [Test]
        public void TestPasswordHashing()
        {
            // Arrange
            var plainPassword = "TestPassword123!";
            var userAccount = new UserAccount("testuser", "Test", "User", plainPassword);

            // Act
            var hashedPassword = userAccount.PasswordHash;
            var salt = userAccount.PasswordSalt;
            var rehashedPassword = PasswordHasher.HashPassword(plainPassword, salt);

            // Assert
            Assert.That(hashedPassword, Is.Not.EqualTo(plainPassword), "Hashed password should not be the same as plain password.");
            Assert.That(hashedPassword, Is.EqualTo(rehashedPassword), "Rehashed password should match the original hash.");
        }

        [Test]
        public void TestPasswordSaltCreation()
        {
            // Arrange
            var userAccount1 = new UserAccount("testuser1", "Test", "User", "Password123!");
            var userAccount2 = new UserAccount("testuser2", "Test", "User", "Password123!");

            // Act
            var salt1 = userAccount1.PasswordSalt;
            var salt2 = userAccount2.PasswordSalt;

            // Assert
            Assert.That(salt1, Is.Not.EqualTo(salt2), "Salts for different user accounts should be unique.");
        }

        [Test]
        [TestCase(null, false)] // null användarnamn bör inte vara giltigt
        [TestCase("", false)] // tomt användarnamn bör inte vara giltigt
        [TestCase("aaaaa", false)] // för kort användarnamn (mindre än 6 tecken)
        [TestCase("abbbbb", true)] // giltigt användarnamn (minst 6 tecken)        
        
        public void TestValidUserName(string userName, bool isValid)
        {
            // Act
            bool result = true;
            try
            {
                var userAccount = new UserAccount(userName, "Test", "User", "Password123!");
            }
            catch (Exception)
            {
                // En exception antyder att skapandet inte är giltigt enligt våra [Required]-attribut
                result = false;
            }

            // Assert
            Assert.That(result, Is.EqualTo(isValid));
        }

        [Test]
        [TestCase(null, false)] // null firstName ej giltigt
        [TestCase("", false)] // tomt firstName ej giltigt
        [TestCase("a", false)] // kortare firstName ej giltigt
        [TestCase("aa", true)] // tillräckligt långt firstName giltigt
        [TestCase("åke", true)] // innehåller giltig specialbokstav, firstName giltigt
        [TestCase("a a", false)] // förnamnet innehåller ett mellanslag, ej giltigt
        [TestCase("a8a", false)] // förnamnet innehåller en siffra, ej giltigt
        [TestCase("a!a", false)] // förnamnet innehåller ett specialtecken, ej giltigt

        public void TestValidFirstName(string firstName, bool isValid) 
        {
            // Act
            bool result = true;
            try
            {
                var userAccount = new UserAccount("username", firstName, "User", "Password123!");
            }
            catch (Exception)
            {
                result= false;
            }

            // Assert

            Assert.That(result, Is.EqualTo(isValid));
        }

        [Test]
        [TestCase(null, false)] // null lastName, ej giltigt
        [TestCase("", false)] // tomt lastName, ej giltigt
        [TestCase("a", false)] // kortare lastName, ej giltigt
        [TestCase("aa", true)] // tillräckligt långt lastName, giltigt
        [TestCase("örn", true)] // innehåller giltig specialbokstav, lastName giltigt
        [TestCase("a a", false)] // efternamnet innehåller ett mellanslag, ej giltigt
        [TestCase("a8a", false)] // efternamnet innehåller en siffra, ej giltigt
        [TestCase("a!a", false)] // efternamnet innehåller ett specialtecken, ej giltigt

        public void TestValidLastName(string lastName, bool isValid)
        {
            // Act
            bool result = true;
            try
            {
                var userAccount = new UserAccount("username", "firstName", lastName, "Password123!");
            }
            catch (Exception)
            {
                result = false;
            }

            // Assert

            Assert.That(result, Is.EqualTo(isValid));
        }
    }
}