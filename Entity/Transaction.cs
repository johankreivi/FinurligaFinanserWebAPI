using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public enum TransactionType { Deposit, Withdrawal, Transaction }
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public int ReceivingAccountNumber { get; private set; }
        public int SendingAccountNumber { get; private set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Transaktionen måste vara minst 1 kr.")]
        public decimal Amount { get; private set; }
        public DateTime TimeStamp { get; private set; }                
        [Required]
        public TransactionType Type { get; private set; }
        [ForeignKey("BankAccount")]
        public int BankAccountId { get; private set; }
        public virtual BankAccount BankAccount { get; private set; }
        
        // Message = Frivillig kommentar att skicka med vid en transaktion
        public string Message {  get; private set; }                

        public Transaction(int receivingAccountNumber, int sendingAccountNumber, decimal amount, TransactionType type, string message = "")
        {
            ReceivingAccountNumber = receivingAccountNumber;
            SendingAccountNumber = sendingAccountNumber;
            Amount = amount;
            TimeStamp = DateTime.Now;
            Type = type;
            Message = message;
        }
    }
}
