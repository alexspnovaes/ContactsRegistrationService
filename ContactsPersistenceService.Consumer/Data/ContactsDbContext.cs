using ContactsPersistenceService.Consumer.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactsPersistenceService.Consumer.Data
{
    public class ContactsDbContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }
        public ContactsDbContext(DbContextOptions<ContactsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.PhoneNumber).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Ddd).IsRequired();
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
