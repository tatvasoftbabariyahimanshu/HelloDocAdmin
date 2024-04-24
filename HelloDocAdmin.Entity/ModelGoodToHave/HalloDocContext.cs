using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HelloDocAdmin.Entity.ModelGoodToHave;

public partial class HalloDocContext : DbContext
{
    public HalloDocContext()
    {
    }

    public HalloDocContext(DbContextOptions<HalloDocContext> options)
        : base(options)
    {
    }

    public virtual DbSet<PayrateByProvider> PayrateByProviders { get; set; }

    public virtual DbSet<PayrateCategory> PayrateCategories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Server=localhost;Port=5432;User Id=postgres;Password=%^78TYui;Database=HalloDoc;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PayrateByProvider>(entity =>
        {
            entity.HasKey(e => e.PayrateId).HasName("PayrateByProvider_pkey");

            entity.ToTable("PayrateByProvider");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedBy).HasColumnType("character varying");
            entity.Property(e => e.ModifiedBy).HasColumnType("character varying");
            entity.Property(e => e.ModifiedDate).HasColumnType("character varying");
            entity.Property(e => e.PhysicianId).HasColumnName("PhysicianID");

            entity.HasOne(d => d.Category).WithMany(p => p.PayrateByProviders)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("PayrateByProvider_CategoryID_fkey");
        });

        modelBuilder.Entity<PayrateCategory>(entity =>
        {
            entity.HasKey(e => e.PayrateCategoryId).HasName("PayrateCategory_pkey");

            entity.ToTable("PayrateCategory");

            entity.Property(e => e.PayrateCategoryId)
                .HasDefaultValueSql("nextval('\"PayrateCategory_PayretID_seq\"'::regclass)")
                .HasColumnName("PayrateCategoryID");
            entity.Property(e => e.CategoryName).HasColumnType("character varying");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
