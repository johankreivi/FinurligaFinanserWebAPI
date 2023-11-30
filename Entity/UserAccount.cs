using System.ComponentModel.DataAnnotations;

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

        public byte[] PasswordSalt { get; private set; } = new byte[8];

        public string PasswordHash { get; private set; } = string.Empty;

        public List<BankAccount>? BankAccounts { get; set; }

        // EF kräver en parameterlös konstruktör
        private UserAccount() { }

        public UserAccount(string userName, string firstName, string lastName, byte[] passwordSalt, string passwordHash)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;            
            PasswordSalt = passwordSalt;
            PasswordHash = passwordHash;
            BankAccounts = new();
        }
    }
}