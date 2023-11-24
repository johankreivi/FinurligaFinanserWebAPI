using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Helpers
{
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
