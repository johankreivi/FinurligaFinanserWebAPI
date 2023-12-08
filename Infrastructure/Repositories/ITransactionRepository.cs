using Entity;
using Infrastructure.Enums;

namespace Infrastructure.Repositories
{
    public interface ITransactionRepository
    {
        public Task<Transaction?> Deposit(Transaction transaction);
        public Task<(Transaction?, TransactionStatus)> CreateTransaction(Transaction transaction);
        public Task<Transaction?> GetTransaction(int id);
        Task<IEnumerable<Transaction?>> GetTransactionsByBankAccountId(int id);
    }
}
