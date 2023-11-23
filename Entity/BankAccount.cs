using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class BankAccount
    {
        [Key]
        public int Id { get; set; }

        // AccountNumber genereras automatiskt av API:t och ska vara 10 siffror.
        public int AccountNumber { get; private set; }
        public string NameOfAccount { get; private set; } = string.Empty;
        public decimal Balance { get; private set; }
        
        // UserAccountId skickas med från FE. Det ska vara samma värde som Id från Username.
        [ForeignKey("UserAccount")]
        [Required]
        public int UserAccountId { get; private set; }
        public virtual List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public BankAccount(int accountNumber, string nameOfAccount, int userAccountId, decimal initialBalance = 0)
        {
            AccountNumber = accountNumber;
            NameOfAccount = nameOfAccount;            
            UserAccountId = userAccountId;
            Balance = initialBalance;
        }
    }
}
