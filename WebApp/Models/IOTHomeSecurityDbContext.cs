using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace WebApp.Models
{
    public partial class IOTHomeSecurityDbContext : DbContext
    {
        public IOTHomeSecurityDbContext() { }
        public IOTHomeSecurityDbContext(DbContextOptions<IOTHomeSecurityDbContext> options) : base(options) { }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=IOTHomeSecurityGlobal;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                // Set the property DateRegisterd to have type datetime in the database
                entity.Property(e => e.DateRegisterd).HasColumnType("datetime");

                // Set the property Email to have type nvarchar(256) in the database
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                // Set the property Password to have type nvarchar(256) in the database
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(256);

                // Set the property Salt to have type nvarchar(16) in the database
                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(16);

                // Set the property UserName to have type nvarchar(128) in the database
                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(128);
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}