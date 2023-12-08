using Entity;

namespace FinurligaFinanserWebAPI.DtoModels.TransactionDTOs
{
    public class DepositConfirmationDTO
    {
        public TransactionType Type { get; set; }
        public int ReceivingAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime TimeStamp { get; set; }
        public decimal AccountBalance { get; set; }     
    }
}
