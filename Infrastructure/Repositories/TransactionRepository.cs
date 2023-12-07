using Entity;
using Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public partial class TransactionRepository : ITransactionRepository
    {
        private readonly DataContext _dataContext;
        public TransactionRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<(Transaction?, TransactionStatus)> CreateTransaction(Transaction transaction)
        {
            if (!ValidateAmount(transaction.Amount)) return (null, TransactionStatus.Invalid_Amount);            

            if (!DoesBankAccountExist(transaction.SendingAccountNumber)) return (null, TransactionStatus.BankAccount_Not_Found);
            if (!DoesBankAccountExist(transaction.ReceivingAccountNumber)) return (null, TransactionStatus.BankAccount_Not_Found);

            var bankAccountSender = GetBankAccount(transaction.SendingAccountNumber);
            var bankAccountReceiver = GetBankAccount(transaction.ReceivingAccountNumber);

            var newBalanceSender = SubtractFromBankAccount(bankAccountSender, transaction.Amount);
            if (newBalanceSender == null) return (null, TransactionStatus.Insufficient_Funds);

            var newBalanceReceiver = AddToBankAccount(bankAccountReceiver, transaction.Amount);
            
            var transactionForSender = new Transaction(transaction.ReceivingAccountNumber, 
                                                    transaction.SendingAccountNumber,
                                                    transaction.Amount, 
                                                    transaction.Message);
            transactionForSender.AccountBalance = (decimal)newBalanceSender;
            transactionForSender.Type = TransactionType.Transaction;
            transactionForSender.BankAccountId = bankAccountSender.Id;

            _dataContext.Transactions.Add(transactionForSender);
            await _dataContext.SaveChangesAsync();

            /////////////////////////////////////////////////////////////

            var transactionForReceiver = new Transaction(transaction.ReceivingAccountNumber,
                                                    transaction.SendingAccountNumber,
                                                    transaction.Amount,
                                                    transaction.Message);
            transactionForReceiver.AccountBalance = newBalanceReceiver;
            transactionForReceiver.Type = TransactionType.Transaction;
            transactionForReceiver.BankAccountId = bankAccountReceiver.Id;

            _dataContext.Transactions.Add(transactionForReceiver);
            await _dataContext.SaveChangesAsync();

            return (transactionForSender, TransactionStatus.Success);  
        }
        public async Task<Transaction?> GetTransaction(int id) => await _dataContext.Transactions.FindAsync(id);        

        public async Task<Transaction?> Deposit(Transaction transaction)
        {
            if (!DoesBankAccountExist(transaction.ReceivingAccountNumber)) return null;
            var bankAccount = GetBankAccount(transaction.ReceivingAccountNumber);

            var depositAmount = transaction.Amount;
            transaction.BankAccountId = bankAccount.Id;
            var newBalance = AddToBankAccount(bankAccount, depositAmount);
            transaction.AccountBalance = newBalance;

            _dataContext.Transactions.Add(transaction);
            await _dataContext.SaveChangesAsync();

            return transaction;
        }

        #region Private Methods
        private BankAccount GetBankAccount(int? accountNumber) => _dataContext.BankAccounts.First(x => x.AccountNumber == accountNumber);
        private bool DoesBankAccountExist(int? bankAccountNumber)
        {
            var bankAccount = _dataContext.BankAccounts.FirstOrDefault(x => x.AccountNumber == bankAccountNumber);
            if (bankAccount == null) return false;
            return true;
        }
        private bool ValidateAmount(decimal amount)
        {
            if (amount <= 0) return false;
            return true;
        }
        private decimal AddToBankAccount(BankAccount receivingBankAccount, decimal amountToAdd)
        {
            receivingBankAccount.Balance += amountToAdd;
            _dataContext.SaveChanges();

            return receivingBankAccount.Balance;
        }
        private decimal? SubtractFromBankAccount(BankAccount sendingBankAccount, decimal amountToRemove)
        {
            if (sendingBankAccount.Balance < amountToRemove) return null;

            sendingBankAccount.Balance -= amountToRemove;
            _dataContext.SaveChanges();

            return sendingBankAccount.Balance;
        }

        
        #endregion


    }
}
