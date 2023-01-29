using GlobalServer.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace GlobalServer.DAL
{

    public partial class GlobalServerDbContext : DbContext
    {
        private string _connectionString;
        public GlobalServerDbContext(string connectionString)
        {

        }
        public virtual DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

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