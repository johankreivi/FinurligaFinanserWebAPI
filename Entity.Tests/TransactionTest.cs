using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace Entity.Tests
{
    [TestFixture]
    public class TransactionTest
    {
        private Transaction _transaction;

        [SetUp]
        public void Setup()
        {
            _transaction = new Transaction(123456789, 123456789, 100, "Test");
        }

        [Test]
        public void Transaction_Constructor_Test_BankAccountId()
        {
            
            var transaction = new Transaction(123456789, 123456789, 100, "Test")
            {
                Id = 1,
                BankAccountId = 1,
                AccountBalance = 100,
                Type = TransactionType.Transaction
            };

            var bankAccount = new BankAccount(123456789, "Test", 100, 123456789) { Id = 1 };
            transaction.BankAccount = bankAccount;

            Assert.That(transaction.BankAccountId, Is.EqualTo(bankAccount.Id));
            Assert.That(transaction.BankAccount, Is.EqualTo(bankAccount));
            Assert.That(transaction.AccountBalance, Is.EqualTo(100));
            Assert.That(transaction.Type, Is.EqualTo(TransactionType.Transaction));
            Assert.That(transaction.Id, Is.EqualTo(1));
        }
    }
}
