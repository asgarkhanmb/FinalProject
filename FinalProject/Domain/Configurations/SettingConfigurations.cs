using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Domain.Configurations
{
    public class SettingConfigurations : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.Property(m => m.Title).IsRequired().HasMaxLength(20);
            builder.Property(m => m.Phone).IsRequired().HasMaxLength(50);
            builder.Property(m => m.Logo).IsRequired();
        }
    }
}
