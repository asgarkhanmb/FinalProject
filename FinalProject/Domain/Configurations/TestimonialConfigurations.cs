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
    public class TestimonialConfigurations : IEntityTypeConfiguration<Testimonial>
    {
        public void Configure(EntityTypeBuilder<Testimonial> builder)
        {
            builder.Property(m => m.Title).IsRequired().HasMaxLength(30);
            builder.Property(m => m.Description).IsRequired().HasMaxLength(300);
            builder.Property(m => m.FullName).IsRequired().HasMaxLength(50);
            builder.Property(m => m.City).IsRequired().HasMaxLength(50);
            builder.Property(m => m.Image).IsRequired();
        }
    }
}
