using _8Boys.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace _8Boys.Context
{
    public class _8BoysContext : IdentityDbContext<ApplicationUser>
    {
        public _8BoysContext(DbContextOptions<_8BoysContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CityShipping> CityShippings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.SeedRole();
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(_8BoysContext).Assembly);

            // Seed roles and other model data via DataSeeder extensions
        }

    }
}
