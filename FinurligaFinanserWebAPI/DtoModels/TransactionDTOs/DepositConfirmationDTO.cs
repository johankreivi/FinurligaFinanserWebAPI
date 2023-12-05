using Entity;

namespace FinurligaFinanserWebAPI.DtoModels.TransactionDTOs
{
    public class DepositConfirmationDTO
    {
        public TransactionType Type { get; set; }
        public int ReceivingAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime TimeStamp { get; set; }
        public DepositConfirmationDTO(TransactionType transactionType, int receivingAccountNumber, decimal amount, DateTime timeStamp)
        {
            Type = transactionType;
            ReceivingAccountNumber = receivingAccountNumber;
            Amount = amount;
            TimeStamp = DateTime.Now;
        }
    }
}
