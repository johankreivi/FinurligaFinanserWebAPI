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
                Assert.That(_bankAccount.AccountNumber, Is.EqualTo(1234567890));
                Assert.That(_bankAccount.NameOfAccount, Is.EqualTo(""));
                Assert.That(_bankAccount.Balance, Is.EqualTo(0));
            });

            Assert.Multiple(() =>
            {
                Assert.That(_bankAccount.Id, Is.EqualTo(0));
                Assert.That(_bankAccount.UserAccountId, Is.EqualTo(1));
                Assert.That(_bankAccount.Transactions, Is.Empty);
            });
        }

        [Test]
        public void TestInitialBalanceEqualToZero() => Assert.That(_bankAccount.Balance, Is.EqualTo(0));              
    }
}
