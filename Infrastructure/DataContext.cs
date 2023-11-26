using Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(50);
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
                entity.HasMany(e => e.Transactions).WithOne().HasForeignKey(t => t.UserAccountId);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.TimeStamp).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.UserAccountId).IsRequired();
            });
        }
    }
}