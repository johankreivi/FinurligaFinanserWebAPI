using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public partial class TransactionRepository : ITransactionRepository
    {
        private readonly DataContext _dataContext;
        public TransactionRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Transaction> Deposit(Transaction transaction)
        {
            var depositAmount = transaction.Amount;
            var bankAccountId = await _dataContext.BankAccounts
                .Where(x => x.AccountNumber == transaction.ReceivingAccountNumber)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (bankAccountId != default)
            {
                var bankAccount = await _dataContext.BankAccounts
                        .FirstOrDefaultAsync(x => x.Id == bankAccountId);
                transaction.BankAccountId = bankAccountId;
                transaction.BankAccount = bankAccount;

                if (bankAccount != null)
                {
                    bankAccount.Balance += depositAmount; 

                    _dataContext.Update(bankAccount); 
                    _dataContext.Add(transaction); 
                    await _dataContext.SaveChangesAsync();
                    return transaction;
                }
                if (bankAccount == null)
                {
                    throw new InvalidOperationException("Bankkonto hittades inte.");
                }
                return null;
            }
            else
            {
                return null;
            }            
        }
    }
}
