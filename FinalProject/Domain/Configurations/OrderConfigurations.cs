using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Configurations
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(m => m.FullName).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Phone).IsRequired().HasMaxLength(30);
            builder.Property(m => m.Address).IsRequired().HasMaxLength(300);
            builder.Property(m => m.Note).HasMaxLength(500);
            builder.Property(m => m.Status).IsRequired().HasMaxLength(20);
            builder.Property(m => m.TotalPrice).HasPrecision(18, 2);
            builder.Property(m => m.StripeSessionId).HasMaxLength(200);
            builder.HasIndex(m => m.StripeSessionId);

            builder.HasOne(m => m.AppUser)
                   .WithMany()
                   .HasForeignKey(m => m.AppUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(m => m.ProductName).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Price).HasPrecision(18, 2);

            builder.HasOne(m => m.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(m => m.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Product)
                   .WithMany()
                   .HasForeignKey(m => m.ProductId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
