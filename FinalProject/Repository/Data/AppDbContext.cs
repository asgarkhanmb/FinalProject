﻿using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Repository.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage>ProductImages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactSetting> ContactSettings { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Social> Socials { get; set; }
        public DbSet<Testimonial>Testimonials { get; set; }
        public DbSet<Instagram> Instagrams { get; set; }
        public DbSet<InstagramGallery> InstagramGalleries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseEntity).Assembly);

            base.OnModelCreating(modelBuilder);
        }



    }
}
