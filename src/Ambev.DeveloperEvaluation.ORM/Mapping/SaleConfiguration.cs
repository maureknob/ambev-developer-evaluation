using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.SaleNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(s => s.SaleNumber).IsUnique();

        builder.Property(s => s.SaleDate).IsRequired().HasColumnType("timestamp");
        builder.Property(s => s.CustomerId).IsRequired().HasColumnType("uuid");
        builder.Property(s => s.CustomerName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.BranchId).IsRequired().HasColumnType("uuid");
        builder.Property(s => s.BranchName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(s => s.IsCancelled).IsRequired().HasDefaultValue(false);
        builder.Property(s => s.CreatedAt).IsRequired().HasColumnType("timestamp");
        builder.Property(s => s.UpdatedAt).HasColumnType("timestamp");

        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.Items).HasField("_items");
    }
}

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(i => i.SaleId).IsRequired().HasColumnType("uuid");
        builder.Property(i => i.ProductId).IsRequired().HasColumnType("uuid");
        builder.Property(i => i.ProductName).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Quantity).IsRequired();
        builder.Property(i => i.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(i => i.Discount).IsRequired().HasColumnType("decimal(5,2)");
        builder.Property(i => i.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(i => i.IsCancelled).IsRequired().HasDefaultValue(false);
    }
}
