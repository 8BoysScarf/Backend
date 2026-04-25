using _8Boys.Context;
using _8Boys.Models;
using _8Boys.Repositry;
using System;

namespace _8Boys
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly _8BoysContext _context;

        public IProductRepository Products { get; }
        public IOrderRepository Orders { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<Color> Colors { get; }
        public IGenericRepository<Badge> Badges { get; }
        public IGenericRepository<ProductVariant> ProductVariants { get; }

        public UnitOfWork(
            _8BoysContext context,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IGenericRepository<Category> categories,
            IGenericRepository<Color> colors,
            IGenericRepository<Badge> badges,
            IGenericRepository<ProductVariant> productVariants
            )
        {
            _context = context;

            Products = productRepository;
            Orders = orderRepository;
            Categories = categories;
            Colors = colors;
            Badges = badges;
            ProductVariants = productVariants;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
