
using Entity;

namespace Infrastructure.Repositories
{
    public interface IBankAccountRepository
    {
        Task<BankAccount> CreateBankAccount(string bankAccountName, int userAccountId);
        Task<BankAccount> GetBankAccount(int id);
    }
}
