using Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DataContext() // An empty constructor is required to be able to mock the datacontext while testing.
        {
            
        }

        public virtual DbSet<UserAccount> UserAccounts { get; set; }
    }
}
