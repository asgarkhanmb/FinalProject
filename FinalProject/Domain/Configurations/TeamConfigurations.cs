using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Domain.Configurations
{
    public class TeamConfigurations : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.Property(m => m.Title).IsRequired().HasMaxLength(20);
            builder.Property(m => m.FullName).IsRequired().HasMaxLength(50);
            builder.Property(m => m.Position).IsRequired().HasMaxLength(30);
            builder.Property(m => m.Image).IsRequired();
        }
    }
}
