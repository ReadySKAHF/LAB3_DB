using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Remoting.Contexts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Domain.Data;

public partial class AutoRepairWorkshopContext : DbContext
{
    public AutoRepairWorkshopContext()
    {
    }

    public AutoRepairWorkshopContext(DbContextOptions<AutoRepairWorkshopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarService> CarServices { get; set; }

    public virtual DbSet<CarStatus> CarStatuses { get; set; }

    public virtual DbSet<Mechanic> Mechanics { get; set; }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<RepairOrder> RepairOrders { get; set; }

    public virtual DbSet<Domain.Models.CachedDataService> Services { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connString = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetConnectionString("DBConnection");
        optionsBuilder.UseSqlServer(connString);
    }
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("PK__Cars__68A0340E3DF8E99A");

            entity.HasIndex(e => e.LicensePlate, "UQ__Cars__026BC15C10A1D448").IsUnique();

            entity.Property(e => e.CarId).HasColumnName("CarID");
            entity.Property(e => e.Brand).HasMaxLength(50);
            entity.Property(e => e.ChassisNumber).HasMaxLength(50);
            entity.Property(e => e.Color).HasMaxLength(30);
            entity.Property(e => e.EngineNumber).HasMaxLength(50);
            entity.Property(e => e.LicensePlate).HasMaxLength(20);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Owner).WithMany(p => p.Cars)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Cars_Owners");
        });

        modelBuilder.Entity<CarService>(entity =>
        {
            entity.HasKey(e => e.CarServiceId).HasName("PK__CarServi__2761C728B27DF72B");

            entity.Property(e => e.CarServiceId).HasColumnName("CarServiceID");
            entity.Property(e => e.MechanicId).HasColumnName("MechanicID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Mechanic).WithMany(p => p.CarServices)
                .HasForeignKey(d => d.MechanicId)
                .HasConstraintName("FK_CarServices_Mechanic");

            entity.HasOne(d => d.Order).WithMany(p => p.CarServices)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_CarServices_Order");

            entity.HasOne(d => d.Service).WithMany(p => p.CarServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_CarServices_Service");
        });

        modelBuilder.Entity<CarStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__CarStatu__C8EE2043792B3368");

            entity.ToTable("CarStatus");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Mechanic>(entity =>
        {
            entity.HasKey(e => e.MechanicId).HasName("PK__Mechanic__6B040DD11E75883B");

            entity.Property(e => e.MechanicId).HasColumnName("MechanicID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Qualification).HasMaxLength(50);
            entity.Property(e => e.Salary).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.HasKey(e => e.OwnerId).HasName("PK__Owners__81938598D109E7BC");

            entity.HasIndex(e => e.DriverLicenseNumber, "UQ__Owners__C32FF2609AFD0A63").IsUnique();

            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.DriverLicenseNumber).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A58F21B56FD");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Employee).HasMaxLength(100);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_Payments_Order");
        });

        modelBuilder.Entity<RepairOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__RepairOr__C3905BAF2D1A6723");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CarId).HasColumnName("CarID");
            entity.Property(e => e.MechanicId).HasColumnName("MechanicID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Car).WithMany(p => p.RepairOrders)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK_RepairOrders_Cars");

            entity.HasOne(d => d.Mechanic).WithMany(p => p.RepairOrders)
                .HasForeignKey(d => d.MechanicId)
                .HasConstraintName("FK_RepairOrders_Mechanics");

            entity.HasOne(d => d.Status).WithMany(p => p.RepairOrders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_RepairOrders_Status");
        });

        modelBuilder.Entity<Domain.Models.CachedDataService>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EA121BFABC");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(100);
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
