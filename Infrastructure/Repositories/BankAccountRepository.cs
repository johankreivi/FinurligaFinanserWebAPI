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
            if (!BankAccountValidator.ValidateBankAccountName(bankAccountName)) return Task.FromException<BankAccount>(new Exception("400BankAccountName")).Result;
            if (!BankAccountValidator.ValidateUserAccountId(userAccountId)) return Task.FromException<BankAccount>(new Exception("400UserAccountId")).Result;

            if (!_dataContext.UserAccounts.Any(x => x.Id == userAccountId)) return Task.FromException<BankAccount>(new Exception("404UserAccount")).Result;

            var bankAccount = new BankAccount(BankAccountGenerator.Generate(), bankAccountName, userAccountId);

            _dataContext.BankAccounts.Add(bankAccount);
            await _dataContext.SaveChangesAsync();

            return bankAccount;
        }

        public void GetAllBankAccountsAsync(int v)
        {
            throw new NotImplementedException();
        }

        public async Task<BankAccount> GetBankAccount(int id)
        {
            var bankAccount = await _dataContext.BankAccounts.FindAsync(id);           

            return bankAccount;
        }      
    }
}
