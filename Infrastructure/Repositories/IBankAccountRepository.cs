using Entity;
using Infrastructure.Enums;

namespace Infrastructure.Repositories
{
    public interface IBankAccountRepository
    {
        Task<(BankAccount?, BankAccountValidationStatus)> CreateBankAccount(string bankAccountName, int userAccountId);
        Task<IEnumerable<BankAccount?>?> GetAllBankAccounts(int userAccountId);
        Task<BankAccount?> GetBankAccount(int id);
        Task<(BankAccount?, BankAccountValidationStatus)> DeleteBankAccount(int id);
    }
}
