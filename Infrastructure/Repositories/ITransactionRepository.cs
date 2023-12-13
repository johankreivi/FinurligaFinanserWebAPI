using Entity;
using Infrastructure.Enums;

namespace Infrastructure.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction?> Deposit(Transaction transaction);
        Task<(Transaction?, TransactionStatus)> CreateTransaction(Transaction transaction);
        Task<Transaction?> GetTransaction(int id);
        Task<IEnumerable<Transaction>?> GetTransactionsByBankAccountId(int id);
    }
}
