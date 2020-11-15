using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;

namespace API
{
    public class DataContext : DbContext
    {
        public static string DbPath { set; private get; }

        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Message> Messages { get; set; }
        public DbSet<Models.KeyExchange> KeyExchanges { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (string.IsNullOrEmpty(DbPath))
                throw new FileNotFoundException("DbPath");
            optionsBuilder.UseSqlite($"Filename={DbPath}", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.User>().HasIndex(x => x.ExchangeFingerprint);
            base.OnModelCreating(modelBuilder);
        }
    }
}
