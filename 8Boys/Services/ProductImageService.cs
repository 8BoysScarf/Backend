using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Repositry;
using Microsoft.EntityFrameworkCore;

namespace _8Boys.Services
{
    public class ProductImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductImageDTO>> GetAllAsync()
        {
            var images = await _unitOfWork.ProductVariants.Query()
                .Include(v => v.Images)
                .SelectMany(v => v.Images.Select(i => new ProductImageDTO
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                }))
                .ToListAsync();

            return images;
        }

        public async Task<IEnumerable<ProductImageDTO>> GetByProductAsync(int productId)
        {
            var images = await _unitOfWork.ProductVariants.Query()
                .Where(v => v.ProductId == productId)
                .Include(v => v.Images)
                .SelectMany(v => v.Images.Select(i => new ProductImageDTO
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                }))
                .ToListAsync();

            return images;
        }
    }
}
