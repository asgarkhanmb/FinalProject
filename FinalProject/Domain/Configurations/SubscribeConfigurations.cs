using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Domain.Configurations
{
    public class SubscribeConfigurations : IEntityTypeConfiguration<Subscribe>
    {
        public void Configure(EntityTypeBuilder<Subscribe> builder)
        {
            builder.Property(m => m.Email).IsRequired().HasMaxLength(200);

        }
    }
}
