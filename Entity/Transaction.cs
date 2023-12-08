using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public enum TransactionType { Deposit, Withdrawal, Transaction }
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public int ReceivingAccountNumber { get; set; }
        public int? SendingAccountNumber { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Transaktionen måste vara minst 1 kr.")]
        public decimal Amount { get; set; }
        public DateTime TimeStamp { get; set; }                
        [Required]
        public TransactionType Type { get; set; }
        [ForeignKey("BankAccount")]
        public int BankAccountId { get; set; }
        // EF-Core vill ha detta för relationsbyggandet
        public virtual BankAccount? BankAccount { get; set; }        
        public string? Message {  get; set; }   
        public decimal AccountBalance { get; set; }

        public Transaction(int receivingAccountNumber, int? sendingAccountNumber, decimal amount, string? message)
        {
            ReceivingAccountNumber = receivingAccountNumber;
            SendingAccountNumber = sendingAccountNumber;
            Amount = amount;
            TimeStamp = DateTime.Now;
            Message = message;
        }
    }
}
