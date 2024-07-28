using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace Domain.Configurations
{
    public class ContactSettingConfigurations : IEntityTypeConfiguration<ContactSetting>
    {
        public void Configure(EntityTypeBuilder<ContactSetting> builder)
        {
            builder.Property(m => m.Title).IsRequired().HasMaxLength(20);
            builder.Property(m => m.Image).IsRequired();
        }
    }
}
