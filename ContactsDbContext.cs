using ContactsAPI.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace ContactsAPI
{
    public class ContactsDbContext : DbContext
    {
        public ContactsDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactAddress> ContactsAddresses { get; set; }
        public DbSet<ContactPhone> ContactsPhones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Contact>()
                .HasIndex(p => p.Email)
                .IsUnique();
            modelBuilder
                .Entity<ContactAddress>()
                .HasOne<Contact>()
                .WithMany(c => c.Addresses)
                .HasForeignKey("ContactId")
                .IsRequired();
            modelBuilder
                .Entity<ContactPhone>()
                .HasOne<Contact>()
                .WithMany(c => c.Phones)
                .HasForeignKey("ContactId")
                .IsRequired();
        }
    }
}