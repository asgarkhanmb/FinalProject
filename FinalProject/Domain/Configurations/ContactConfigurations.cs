using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Domain.Configurations
{
    public class ContactConfigurations : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.Property(m => m.Name).IsRequired().HasMaxLength(20);
            builder.Property(m => m.Email).IsRequired().HasMaxLength(200);
            builder.Property(m => m.Message).IsRequired().HasMaxLength(200);
        }
    }
}
