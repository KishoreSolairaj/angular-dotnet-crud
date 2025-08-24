using EmployeeRegistration.Server.Model;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
namespace EmployeeRegistration.Server
{
    public class AppDbContext : DbContext
    {
      public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Employee configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee_Mst");
                entity.HasKey(e => e.EmployeeId);
                entity.Property(e => e.EmployeeId).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.MobileNum).IsUnique();

                entity.HasOne(e => e.State)
                    .WithMany(s => s.Employees)
                    .HasForeignKey(e => e.StateId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Country)
                    .WithMany(c => c.Employees)
                    .HasForeignKey(e => e.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Country configuration
            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country_Mst");
                entity.HasKey(c => c.CountryId);
                entity.Property(c => c.CountryId).ValueGeneratedOnAdd();
            });

            // State configuration
            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("State_Mst");
                entity.HasKey(s => s.StateId);
                entity.Property(s => s.StateId).ValueGeneratedOnAdd();

                entity.HasOne(s => s.Country)
                    .WithMany(c => c.States)
                    .HasForeignKey(s => s.CountryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
