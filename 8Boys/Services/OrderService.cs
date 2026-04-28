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
            var address = await _unitOfWork.Addresses.GetByIdAsync(addressId);
            var city =await _unitOfWork.CityShippings.Query().FirstOrDefaultAsync(c => c.City == address.City);
            if (city==null) { 
                return -1;
            }

            var order = new Order
            {
                UserId = userId,
                AddressId = addressId,
                Status = OrderStatus.Pending,
                Items = new List<OrderItem>(),
                ShippingPrice = city.Price
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

        public async Task<OrderDetailsDTO?> GetOrderDetailsAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return null;

            var full = await _unitOfWork.Orders.Query()
                .Where(o => o.Id == orderId)
                .Include(o => o.User)
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
                CustomerId = full.UserId,
                CustomerName = full.User?.Name,
                CustomerProfile = full.User?.ProfilePictureUrl,
                TotalAmount = full.TotalAmount,
                Status = full.Status.ToString(),
                ShippingPrice = full.ShippingPrice,
                Items = full.Items.Select(i => new OrderItemDTO
                {
                    ProductId = i.ProductVariant?.ProductId ?? 0,
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
                Status = o.Status.ToString(),
                ShippingPrice = o.ShippingPrice
            });
        }

        // Admin: list all orders with optional filters
        public async Task<PagedResult<OrderSummaryDTO>> GetAllOrdersAsync(int page = 1, int pageSize = 20, decimal? minPrice = null, decimal? maxPrice = null, OrderStatus? status = null)
        {
            var query = _unitOfWork.Orders.Query().AsQueryable();

            if (minPrice.HasValue) query = query.Where(o => o.TotalAmount >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(o => o.TotalAmount <= maxPrice.Value);
            if (status.HasValue) query = query.Where(o => o.Status == status.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderSummaryDTO
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),
                    ShippingPrice = o.ShippingPrice
                })
                .ToListAsync();

            return new PagedResult<OrderSummaryDTO>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = newStatus;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
