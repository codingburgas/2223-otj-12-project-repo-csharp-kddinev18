﻿using AuthAPI.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.DAL.Data
{
    public partial class AuthAPIDbContext : DbContext, IAuthAPIDbContext
    {
        public AuthAPIDbContext() { }
        public AuthAPIDbContext(DbContextOptions<AuthAPIDbContext> options) : base(options) { }
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
                entity.Property(e => e.Id).HasColumnType("nvarchar(36)");
                entity.HasIndex(e => e.Id);

                // Set the property DateRegisterd to have type datetime in the database
                entity.Property(e => e.DateRegisterd).HasColumnType("datetime2(7)");

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

                entity.HasIndex(e => e.UserName)
                    .IsUnique();
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}
