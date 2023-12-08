using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using System.Linq;

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
            Assert.NotNull(_dataContext.UserAccounts);
        }

        [Test]
        public void BankAccountsDbSet_Initialized_NotNull()
        {
            Assert.NotNull(_dataContext.BankAccounts);
        }

        [Test]
        public void TransactionsDbSet_Initialized_NotNull()
        {
            Assert.NotNull(_dataContext.Transactions);
        }

        [Test]
        public void OnModelCreating_UserAccount_Test_all_properties_are_configured()
        {
            var entity = _dataContext.Model.FindEntityType(typeof(Entity.UserAccount));

            Assert.AreEqual("UserAccount", entity.GetTableName());
            Assert.AreEqual("Id", entity.FindPrimaryKey().Properties.First().Name);

            Assert.AreEqual(6, entity.GetProperties().Count());
        }

        [Test]
        public void OnModelCreating_BankAccount_Test_all_properties_are_configured()
        {
            var entity = _dataContext.Model.FindEntityType(typeof(Entity.BankAccount));

            Assert.AreEqual("BankAccount", entity.GetTableName());
            Assert.AreEqual("Id", entity.FindPrimaryKey().Properties.First().Name);

            Assert.AreEqual(5, entity.GetProperties().Count());
        }

        [Test]
        public void OnModelCreating_Transaction_Test_all_properties_are_configured()
        {
            var entity = _dataContext.Model.FindEntityType(typeof(Entity.Transaction));

            Assert.AreEqual("Transaction", entity.GetTableName());
            Assert.AreEqual("Id", entity.FindPrimaryKey().Properties.First().Name);

            Assert.AreEqual(9, entity.GetProperties().Count());
        }
        // Additional tests...

        [TearDown]
        public void Teardown()
        {
            _dataContext.Dispose();
        }
    }
}
