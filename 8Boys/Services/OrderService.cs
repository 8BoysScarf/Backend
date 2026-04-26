using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Repositry;
using Microsoft.EntityFrameworkCore;

namespace _8Boys.Services
{
    public class OrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateOrderFromCartAsync(string userId, int addressId)
        {
            // reuse existing CartService logic or replicate minimal code here
            var cart = await _unitOfWork.Carts.GetUserCartAsync(userId);
            if (cart == null || !cart.Items.Any()) throw new InvalidOperationException("Cart is empty");

            var order = new Order
            {
                UserId = userId,
                AddressId = addressId,
                Status = "Pending",
                Items = new List<OrderItem>()
            };

            decimal total = 0;
            foreach (var item in cart.Items)
            {
                var variant = await _unitOfWork.ProductVariants.GetByIdAsync(item.ProductVariantId);
                if (variant == null) throw new InvalidOperationException("Product variant not found");

                order.Items.Add(new OrderItem
                {
                    ProductVariantId = variant.Id,
                    Quantity = item.Quantity,
                    Price = variant.Price
                });

                total += variant.Price * item.Quantity;
            }

            order.TotalAmount = total;
            await _unitOfWork.Orders.AddAsync(order);

            // clear cart
            foreach (var item in cart.Items.ToList())
            {
                _unitOfWork.CartItems.Remove(item);
            }

            await _unitOfWork.SaveChangesAsync();
            return order.Id;
        }

        public async Task<OrderDetailsDTO?> GetOrderDetailsAsync(int orderId, string userId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || order.UserId != userId) return null;

            var full = await _unitOfWork.Orders.Query()
                .Where(o => o.Id == orderId)
                .Include(o => o.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(v => v.Product)
                .Include(o => o.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(v => v.Images)
                .FirstOrDefaultAsync();

            if (full == null) return null;

            return new OrderDetailsDTO
            {
                Id = full.Id,
                TotalAmount = full.TotalAmount,
                Status = full.Status,
                Items = full.Items.Select(i => new OrderItemDTO
                {
                    ProductVariantId = i.ProductVariantId,
                    ProductName = i.ProductVariant?.Product?.Name,
                    VariantCode = i.ProductVariant?.Code,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Thumbnail = i.ProductVariant?.Images?.FirstOrDefault()?.ImageUrl
                })
            };
        }

        public async Task<IEnumerable<OrderSummaryDTO>> GetUserOrdersAsync(string userId)
        {
            var orders = await _unitOfWork.Orders.GetUserOrdersAsync(userId);
            return orders.Select(o => new OrderSummaryDTO
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                Status = o.Status
            });
        }
    }
}
