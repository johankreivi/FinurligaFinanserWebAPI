using Entity;

namespace FinurligaFinanserWebAPI.DtoModels.TransactionDTOs
{
    public class TransactionConfirmationDTO
    {
        public int ReceivingAccountNumber { get; set; }
        public int SendingAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime TimeStamp { get; set; }
        public TransactionType Type { get; set; }
        public string? Message { get; set; }
        public decimal AccountBalance { get; set; }
    }
}
