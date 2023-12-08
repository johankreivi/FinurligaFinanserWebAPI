namespace FinurligaFinanserWebAPI.DtoModels.TransactionDTOs
{
    public class PostTransactionDTO
    {
        public int SendingAccountNumber { get; set; }
        public int ReceivingAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string? Message { get; set; }
    }
}
