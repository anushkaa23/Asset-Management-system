using Microsoft.EntityFrameworkCore;
using AssetManagement.Data.Entities;

namespace AssetManagement.Data.Context
{
    public class AssetDbContext : DbContext
    {
        public AssetDbContext(DbContextOptions<AssetDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure for both SQL Server and PostgreSQL
            // Employee configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Department).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Designation).HasMaxLength(50).IsRequired();
            });

            // Asset configuration
            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasIndex(a => a.SerialNumber);
                
                entity.Property(a => a.AssetName).HasMaxLength(100).IsRequired();
                entity.Property(a => a.AssetType).HasMaxLength(50).IsRequired();
                entity.Property(a => a.MakeModel).HasMaxLength(100);
                entity.Property(a => a.SerialNumber).HasMaxLength(100);
                entity.Property(a => a.Specifications).HasMaxLength(1000);
            });

            // Assignment configuration
            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasOne(a => a.Asset)
                    .WithMany(asset => asset.Assignments)
                    .HasForeignKey(a => a.AssetId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Employee)
                    .WithMany(emp => emp.Assignments)
                    .HasForeignKey(a => a.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(a => a.Notes).HasMaxLength(500);
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                
                entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
                entity.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
                entity.Property(u => u.FullName).HasMaxLength(100);
            });
        }
    }
}