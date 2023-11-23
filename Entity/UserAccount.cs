using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Entity
{
    public partial class UserAccount
    {
        public int Id { get; set; }

        [Required]
        [MinLength(6)]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [MinLength(2)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [MinLength(2)]
        public string LastName { get; set; } = string.Empty;

        // Här sparar vi Saltet till databasen, det kommer behövas vid inloggning ihop med userName och password.
        public byte[] PasswordSalt { get; private set; }

        private string _passwordHash;
        public string PasswordHash
        {
            get { return _passwordHash; }
            private set { _passwordHash = value; }
        }

        public List<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

        // EF kräver en parameterlös konstruktör
        private UserAccount() { }

        public UserAccount(string userName, string firstName, string lastName, string password)
        {
            ValidateUserName(userName);
            ValidateFirstName(firstName);
            ValidateLastName(lastName);
            ValidatePassword(password);

            UserName = userName;
            FirstName = firstName;
            LastName = lastName;

            // Vid skapandet av ett nytt UserAccount objekt, generera ett salt och hash för lösenordet.
            PasswordSalt = PasswordHasher.GenerateSalt();
            PasswordHash = PasswordHasher.HashPassword(password, PasswordSalt);
        }

        private void ValidateUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName) || userName.Length < 6)
            {
                throw new ArgumentException("Username must be at least 6 letters.", nameof(userName));
            }
        }

        private void ValidateFirstName(string firstName)
        {
            if (string.IsNullOrEmpty(firstName) || firstName.Length < 2)
            {
                throw new ArgumentException("First name must be at least 2 letters.", nameof(firstName));
            }
            if (!CheckForInvalidLetters().IsMatch(firstName))
            {
                throw new ArgumentException("First name contains invalid characters.", nameof(firstName));
            }
        }

        private void ValidateLastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName) || lastName.Length < 2)
            {
                throw new ArgumentException("Last name must be at least 2 letters.", nameof(lastName));
            }
            if (!CheckForInvalidLetters().IsMatch(lastName))
            {
                throw new ArgumentException("Last name contains invalid characters.", nameof(lastName));
            }
        }

        private void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || !PasswordIsValid().IsMatch(password))
            {
                throw new ArgumentException("Password must meet complexity requirements.", nameof(password));
            }
        }

        [GeneratedRegex("^[a-zA-ZäöåÄÖÅ]+$")]
        private static partial Regex CheckForInvalidLetters();
        [GeneratedRegex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).+$")]
        private static partial Regex PasswordIsValid();
    }

    public static class PasswordHasher
    {
        // Skapar en hash av ett lösenord
        public static string HashPassword(string password, byte[] salt)
        {
            var saltedPassword = MergeSaltAndPassword(password, salt);
            var hash = SHA256.HashData(saltedPassword);
            return Convert.ToBase64String(hash);
        }

        // Skapar ett nytt salt
        public static byte[] GenerateSalt()
        {
            using var random = new RNGCryptoServiceProvider();
            var salt = new byte[32]; // 256 bits
            random.GetNonZeroBytes(salt);
            return salt;
        }

        // Slår samman salt och lösenord
        private static byte[] MergeSaltAndPassword(string password, byte[] salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = new byte[salt.Length + passwordBytes.Length];
            Buffer.BlockCopy(salt, 0, saltedPassword, 0, salt.Length);
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, salt.Length, passwordBytes.Length);
            return saltedPassword;
        }
    }
}
