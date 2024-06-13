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

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Guid).HasName("PRIMARY");

            entity.ToTable("order");

            entity.HasIndex(e => e.TruckNumber, "TruckNumber_idx");

            entity.HasIndex(e => e.UserLogin, "UserLogin_idx");

            entity.Property(e => e.Guid).HasMaxLength(36);
            entity.Property(e => e.Address).HasMaxLength(45);
            entity.Property(e => e.CargoVolume).HasPrecision(56, 28);
            entity.Property(e => e.CargoWeight).HasPrecision(56, 28);
            entity.Property(e => e.DateBegin).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.Price).HasPrecision(56, 28);
            entity.Property(e => e.TruckNumber).HasMaxLength(9);
            entity.Property(e => e.UserLogin).HasMaxLength(45);

           entity.HasOne(d => d.TruckNumberNavigation).WithMany(p => p.Orders)
               .HasForeignKey(d => d.TruckNumber)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("TruckNumber");

            entity.HasOne(d => d.UserLoginNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserLogin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserLogin");
        });

        modelBuilder.Entity<Truck>(entity =>
        {
            entity.HasKey(e => e.Number).HasName("PRIMARY");

            entity.ToTable("truck");

            entity.HasIndex(e => e.DriverGuid, "DriverGuid_UNIQUE").IsUnique();

            entity.Property(e => e.Number).HasMaxLength(9);
            entity.Property(e => e.DriverGuid).HasMaxLength(36);
            entity.Property(e => e.TypeAdr).HasMaxLength(45);
            entity.Property(e => e.VolumeMax).HasPrecision(56, 28);
            entity.Property(e => e.VolumePrice).HasPrecision(56, 28);
            entity.Property(e => e.WeightMax).HasPrecision(56, 28);
            entity.Property(e => e.WeightPrice).HasPrecision(56, 28);
            entity.Property(e => e.PricePerKilometer).HasPrecision(56, 28);

            entity.HasOne(d => d.Driver).WithOne(p => p.Truck)
                .HasForeignKey<Truck>(d => d.DriverGuid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("DriverGuid");
        });
        modelBuilder.Entity<Truck>().ToTable(b => b.HasCheckConstraint("CHK_TypeAdr",
            "TypeAdr = 'EXII' OR TypeAdr = 'EXIII' OR TypeAdr = 'FL' OR TypeAdr = 'AT' OR TypeAdr = 'MEMU'"));

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Login).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.Password, "Password_UNIQUE").IsUnique();

            entity.Property(e => e.Login).HasMaxLength(45);
            entity.Property(e => e.Contact).HasMaxLength(45);
            entity.Property(e => e.DefaultAddress).HasMaxLength(45);
            entity.Property(e => e.Name).HasMaxLength(45);
            entity.Property(e => e.Password).HasMaxLength(128);
            entity.Property(e => e.DynamicPartOfSalt).HasMaxLength(128);
        });
    }

    public DbSet<Employee> Employees { get; set; } = null!;
}