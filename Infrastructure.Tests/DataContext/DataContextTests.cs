using Microsoft.EntityFrameworkCore;
using Infrastructure;

namespace InfrastructureTests
{
    public class DataContextTests
    {
        private DataContext _dataContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dataContext = new DataContext(options);
        }

        [Test]
        public void UserAccountsDbSet_Initialized_NotNull()
        {
            Assert.That(_dataContext.UserAccounts, Is.Not.Null);
        }

        [Test]
        public void BankAccountsDbSet_Initialized_NotNull()
        {
            Assert.That(_dataContext.BankAccounts, Is.Not.Null);
        }

        [Test]
        public void TransactionsDbSet_Initialized_NotNull()
        {
            Assert.That(_dataContext.Transactions, Is.Not.Null);
        }

        [Test]
        public void OnModelCreating_UserAccount_Test_all_properties_are_configured()
        {
            var entity = _dataContext.Model.FindEntityType(typeof(Entity.UserAccount));

            Assert.That(entity, Is.Not.Null);
            Assert.That(entity.FindPrimaryKey(), Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(entity.GetTableName(), Is.EqualTo("UserAccount"));
                Assert.That(entity.FindPrimaryKey().Properties[0].Name, Is.EqualTo("Id"));

                Assert.That(entity.GetProperties().Count(), Is.EqualTo(6));
            });
        }

        [Test]
        public void OnModelCreating_BankAccount_Test_all_properties_are_configured()
        {
            var entity = _dataContext.Model.FindEntityType(typeof(Entity.BankAccount));

            Assert.That(entity, Is.Not.Null);
            Assert.That(entity.FindPrimaryKey(), Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(entity.GetTableName(), Is.EqualTo("BankAccount"));
                Assert.That(entity.FindPrimaryKey().Properties[0].Name, Is.EqualTo("Id"));

                Assert.That(entity.GetProperties().Count(), Is.EqualTo(5));
            });
        }

        [Test]
        public void OnModelCreating_Transaction_Test_all_properties_are_configured()
        {
            var entity = _dataContext.Model.FindEntityType(typeof(Entity.Transaction));

            Assert.That(entity, Is.Not.Null);
            Assert.That(entity.FindPrimaryKey(), Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(entity.GetTableName(), Is.EqualTo("Transaction"));
                Assert.That(entity.FindPrimaryKey().Properties[0].Name, Is.EqualTo("Id"));

                Assert.That(entity.GetProperties().Count(), Is.EqualTo(9));
            });
        }
        // Additional tests...

        [TearDown]
        public void Teardown()
        {
            _dataContext.Dispose();
        }
    }
}
