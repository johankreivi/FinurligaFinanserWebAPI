using Entity;
using Infrastructure.Helpers;

namespace Infrastructure.Repositories
{
    public partial class BankAccountRepository : IBankAccountRepository
    {
        private readonly DataContext _dataContext;
        public BankAccountRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<BankAccount> CreateBankAccount(string bankAccountName, int userAccountId)
        {
            var bankAccount = new BankAccount(BankAccountGenerator.Generate(), bankAccountName, userAccountId);

            _dataContext.BankAccounts.Add(bankAccount);
            await _dataContext.SaveChangesAsync();

            return bankAccount;
        }

        public async Task<BankAccount> GetBankAccount(int id)
        {
            var bankAccount = await _dataContext.BankAccounts.FindAsync(id);           

            return bankAccount;
        }      
    }
}
