using Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DataContext() { }

        public virtual DbSet<UserAccount> UserAccounts { get; set; }
        public virtual DbSet<BankAccount> BankAccounts { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.UserName).IsUnique();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.HasMany(u => u.BankAccounts).WithOne().HasForeignKey(ba => ba.UserAccountId);
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(b => b.Balance).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.AccountNumber).IsRequired();
                entity.Property(e => e.NameOfAccount).IsRequired();
                entity.HasMany(b => b.Transactions)
                      .WithOne(t => t.BankAccount)
                      .HasForeignKey(t => t.BankAccountId);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.TimeStamp).IsRequired();
                entity.Property(e => e.Type).IsRequired();                
                entity.Property(e => e.Message);                                
            });
        }

        //public override int SaveChanges()
        //{
        //    //var Transaction = Transactions.TableAs.Include(t => t.TableB).FirstOrDefault(t => t.Id == someId);
        //    foreach (var entry in ChangeTracker.Entries<Transaction>())
        //    {
        //        if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
        //        {
        //            // Assuming that the calculation is based on a property of TableB
        //            // Ensure that TableB is loaded or use a different method to obtain the necessary data
        //            var bAccounts = BankAccounts.Find(entry.Entity.BankAccountId);
        //            if (bAccounts != null)
        //            {
        //                // Perform your calculation here
        //                entry.Entity.AccountBalance = bAccounts.Balance + entry.Entity.Amount;
        //            }
        //        }
        //    }

        //    return base.SaveChanges();
        //}
    }
}