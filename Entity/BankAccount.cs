using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class BankAccount
    {
        [Key]
        public int Id { get; set; }
        public int AccountNumber { get; private set; }
        public string NameOfAccount { get; private set; }
        public decimal Balance { get; private set; }
                
        [ForeignKey("UserAccount")]
        [Required]
        public int UserAccountId { get; private set; }
        public List<Transaction>? Transactions { get; set; } = new List<Transaction>();

        public BankAccount(int accountNumber, string nameOfAccount, int userAccountId, decimal balance = 0)
        {
            AccountNumber = accountNumber;
            NameOfAccount = nameOfAccount;
            UserAccountId = userAccountId;
            Balance = balance;
        }
    }
}
