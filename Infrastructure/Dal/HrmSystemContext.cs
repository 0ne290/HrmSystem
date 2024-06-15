using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public sealed class HrmSystemContext : DbContext
{
    public HrmSystemContext(DbContextOptions<HrmSystemContext> options) : base(options) => Database.EnsureCreated();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Guid).HasName("PRIMARY");

            entity.ToTable("driver");

            entity.Property(e => e.Guid).HasMaxLength(36);
            entity.Property(e => e.Name).HasMaxLength(45);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Login).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.Password, "Password_UNIQUE").IsUnique();

            entity.Property(e => e.DynamicPartOfSalt).HasMaxLength(128);
            entity.Property(e => e.Login).HasMaxLength(128);
            entity.Property(e => e.Password).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Contact).HasMaxLength(128);
            entity.Property(e => e.CurrentProjectUrl).HasMaxLength(128);
            entity.Property(e => e.PremiumRate).HasPrecision(56, 28);
            entity.Property(e => e.Premium).HasPrecision(56, 28);
            entity.Property(e => e.PremiumRate).HasPrecision(56, 28);
            entity.Property(e => e.PremiumRate).HasPrecision(56, 28);

           entity.HasOne(e => e.PositionNavigation).WithMany(p => p.Employees)
               .HasForeignKey(d => d.PositionGuid)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("PositionGuid");
        });
    }

    public DbSet<Employee> Employees { get; set; } = null!;
}