using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Tests
{
    [TestFixture]
    public class BankAccountTests
    {
        private BankAccount _bankAccount;

        [SetUp]
        public void Setup()
        {
            _bankAccount = new BankAccount(1234567890, "", 1);
        }

        [Test]
        public void TestBankAccountCreation()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_bankAccount.AccountNumber.Equals(1234567890));
                Assert.That(_bankAccount.AccountNumber.Equals(1234567890));
                Assert.That(_bankAccount.AccountNumber.Equals(1234567890));

            });
        }
    }
}
