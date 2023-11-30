
using Entity;

namespace Infrastructure.Repositories
{
    public interface IBankAccountRepository
    {
        Task<BankAccount> CreateBankAccount(string bankAccountName, int userAccountId);
        void GetAllBankAccountsAsync(int v);
        Task<BankAccount> GetBankAccount(int id);
    }
}
