using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Repositry;
using Microsoft.EntityFrameworkCore;

namespace _8Boys.Services
{
    public class ReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // user can add review only if he has an order with this product/productVariant and order delivered
        public async Task<int> AddReviewAsync(string userId, AddReviewDTO dto)
        {
            // ensure product exists
            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null) throw new InvalidOperationException("Product not found");

            // check orders for user containing product or product variant and with Delivered status
            var hasDelivered = await _unitOfWork.Orders.Query()
                .Where(o => o.UserId == userId && o.Status == OrderStatus.Delivered)
                .AnyAsync(o => o.Items.Any(i => i.ProductVariant.ProductId == dto.ProductId || (dto.ProductVariantId.HasValue && i.ProductVariantId == dto.ProductVariantId.Value)));

            if (!hasDelivered)
                throw new InvalidOperationException("You can review only products that you have received (delivered).");

            // ensure user hasn't already reviewed this product (unique per user+product)
            var exists = await _unitOfWork.Reviews.Query().AnyAsync(r => r.UserId == userId && r.ProductId == dto.ProductId);
            if (exists) throw new InvalidOperationException("You already reviewed this product.");

            var review = new Review
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
            return review.Id;
        }

        // Admin can delete a review
        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var existing = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (existing == null) return false;

            _unitOfWork.Reviews.Remove(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // Get reviews, optionally filtered by productId
        public async Task<IEnumerable<ReviewDTO>> GetAllAsync(int? productId = null)
        {
            var query = _unitOfWork.Reviews.Query()
                .Include(r => r.User)
                .AsQueryable();

            if (productId.HasValue)
                query = query.Where(r => r.ProductId == productId.Value);

            var list = await query
                .OrderByDescending(r => r.Id)
                .Select(r => new ReviewDTO
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserName = r.User != null ? r.User.Name : null,
                    Rating = r.Rating,
                    Comment = r.Comment
                })
                .ToListAsync();

            return list;
        }
    }
}
