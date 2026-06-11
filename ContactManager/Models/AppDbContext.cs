using System;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    
    }

    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Contact>()
            .HasOne(c => c.Department)
            .WithMany(d => d.Contacts)
            .HasForeignKey(c => c.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "IT-отдел" },
            new Department { Id = 2, Name = "Бухгалтерия" }
        );

        modelBuilder.Entity<Contact>().HasData(
            new Contact
            {
                Id = 1,
                FullName = "Иванов Иван Иванович",
                Email = "ivanov@example.com",
                Phone = "+7 (999) 111-22-33",
                IsActive = true,
                DepartmentId = 1,
                CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}

