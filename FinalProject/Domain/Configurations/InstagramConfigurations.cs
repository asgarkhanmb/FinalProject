using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Configurations
{
    public class InstagramConfigurations : IEntityTypeConfiguration<Instagram>
    {
        public void Configure(EntityTypeBuilder<Instagram> builder)
        {
            builder.Property(m => m.Title).IsRequired().HasMaxLength(50);
            builder.Property(m => m.SocialName).IsRequired().HasMaxLength(100);

        }
    }
}
