using Entity;
using Infrastructure.Helpers;
using Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public partial class BankAccountRepository : IBankAccountRepository
    {
        private readonly DataContext _dataContext;
        public BankAccountRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<(BankAccount?, BankAccountValidationStatus)> CreateBankAccount(string bankAccountName, int userAccountId)
        {

            if (!BankAccountValidator.ValidateBankAccountName(bankAccountName)) return ( null, BankAccountValidationStatus.Invalid_BankAccountName);
            if (!BankAccountValidator.ValidateUserAccountId(userAccountId)) return (null, BankAccountValidationStatus.Invalid_UserAccountId);

            if (!_dataContext.UserAccounts.Any(x => x.Id == userAccountId)) return (null, BankAccountValidationStatus.NotFound);

            int bankAccountNumber = GetBankAccountNumber();

            var bankAccount = new BankAccount(bankAccountNumber, bankAccountName, userAccountId);

            _dataContext.BankAccounts.Add(bankAccount);
            await _dataContext.SaveChangesAsync();

            return (bankAccount, BankAccountValidationStatus.Valid);
        }

        public async Task<(BankAccount?, BankAccountValidationStatus)> DeleteBankAccount(int id)
        {
            var bankAccount = await _dataContext.BankAccounts.FindAsync(id);

            if (bankAccount == null) return (null, BankAccountValidationStatus.NotFound);

            if (bankAccount.Balance != 0) return (null, BankAccountValidationStatus.Invalid_amount);

            var result = _dataContext.BankAccounts.Remove(bankAccount);

            await _dataContext.SaveChangesAsync();
            return (bankAccount, BankAccountValidationStatus.Valid);
        }

        public async Task<IEnumerable<BankAccount?>?> GetAllBankAccounts(int userAccountId)
        {
            var bankAccounts = await _dataContext.BankAccounts.Where(x => x.UserAccountId == userAccountId).ToListAsync();
            if (bankAccounts.Count() == 0) return null;

            return bankAccounts;
        }

        public async Task<BankAccount?> GetBankAccount(int id) => await _dataContext.BankAccounts.FindAsync(id);

        #region Private Methods
        private int GetBankAccountNumber()
        {
            int bankAccountNumber;
            bool IsBankAccoutNumberTaken = true;

            do
            {
                bankAccountNumber = BankAccountGenerator.Generate();
                IsBankAccoutNumberTaken = _dataContext.BankAccounts.Any(ba => ba.AccountNumber == bankAccountNumber);

            } while (IsBankAccoutNumberTaken == true);

            return bankAccountNumber;
        }
        #endregion
    }
}
