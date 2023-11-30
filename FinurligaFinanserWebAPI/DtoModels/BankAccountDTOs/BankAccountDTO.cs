using Entity;

namespace FinurligaFinanserWebAPI.DtoModels.BankAccountDTOs
{
    public class BankAccountDTO
    {
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public string NameOfAccount { get; set; }
        public decimal Balance { get; set; }
        // TODO: Ändra till DTO om det krävs
        public List<Transaction>? Transactions { get; set; } = new List<Transaction>();
    }
}
