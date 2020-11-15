using Microsoft.EntityFrameworkCore;
using SecureTalk.Models;
using System.Reflection;

namespace SecureTalk
{
    public class DataContext : DbContext
    {
        public static string DbPath;

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<MessageKey> MessageKeys { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={DbPath}", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageKey>().HasIndex(x => x.AssignedContactId);
            modelBuilder.Entity<Message>().HasIndex(x => x.SenderId);
            modelBuilder.Entity<Message>().HasIndex(x => x.DecryptedSenderId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
