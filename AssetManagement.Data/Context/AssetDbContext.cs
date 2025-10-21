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

            // Employee configuration
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // Asset configuration
            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.SerialNumber);

            // Assignment configuration
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Asset)
                .WithMany(asset => asset.Assignments)
                .HasForeignKey(a => a.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Employee)
            .WithMany(emp => emp.Assignments)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);   // Changed from Restrict

            // User configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
