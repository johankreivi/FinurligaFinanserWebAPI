using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public partial class UserAccount
    {
        public int Id { get; set; }

        [Required]
        [MinLength(6)]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        [MinLength(2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [MinLength(2)]
        public string LastName { get; set; }

        // Här sparar vi Saltet till databasen, det kommer behövas vid inloggning ihop med userName och password.
        public byte[] PasswordSalt { get; private set; }

        public string PasswordHash { get; private set; }
        
        public List<BankAccount> BankAccounts { get; set; }

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
