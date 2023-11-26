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
        [ForeignKey("UserAccount")]
        [Required]
        public int UserAccountId { get; private set; }
        [Required]
        public TransactionType Type { get; private set; }


        public Transaction(int receivingAccountNumber, int sendingAccountNumber, decimal amount, TransactionType type)
        {
            ReceivingAccountNumber = receivingAccountNumber;
            SendingAccountNumber = sendingAccountNumber;
            Amount = amount;
            TimeStamp = DateTime.Now;
            Type = type;
        }
    }
}
