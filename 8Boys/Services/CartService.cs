using _8Boys.Models;
using _8Boys.Repositry;
using Microsoft.EntityFrameworkCore;
using _8Boys.DTOs;

namespace _8Boys.Services
{
    public class CartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetUserCartAsync(userId);
            if (cart != null) return cart;

            cart = new Cart { UserId = userId, Items = new List<CartItem>() };
            await _unitOfWork.Carts.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();
            return cart;
        }

        public async Task AddItemAsync(string userId, int productVariantId, int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");

            var cart = await GetOrCreateCartAsync(userId);

            var existing = cart.Items.FirstOrDefault(i => i.ProductVariantId == productVariantId);
            if (existing != null)
            {
                existing.Quantity += quantity;
                _unitOfWork.CartItems.Update(existing);
            }
            else
            {
                var item = new CartItem { CartId = cart.Id, ProductVariantId = productVariantId, Quantity = quantity };
                await _unitOfWork.CartItems.AddAsync(item);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(string userId, int productVariantId)
        {
            var cart = await _unitOfWork.Carts.GetUserCartAsync(userId);
            if (cart == null) return;

            var existing = cart.Items.FirstOrDefault(i => i.ProductVariantId == productVariantId);
            if (existing == null) return;

            _unitOfWork.CartItems.Remove(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Cart?> GetCartAsync(string userId)
        {
            return await _unitOfWork.Carts.GetUserCartAsync(userId);
        }

        public async Task<IEnumerable<CartItemDTO>> GetCartItemsDtoAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetUserCartAsync(userId);
            if (cart == null) return Enumerable.Empty<CartItemDTO>();

            // Ensure product variant and product are loaded
            await _unitOfWork.Carts.Query()
                .Where(c => c.Id == cart.Id)
                .Include(c => c.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(v => v.Product)
                .Include(c => c.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(v => v.Images)
                .Include(c => c.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(v => v.Color)
                .FirstOrDefaultAsync();

            var list = cart.Items.Select(i => new CartItemDTO
            {
                Id = i.Id,
                ProductVariantId = i.ProductVariantId,
                Quantity = i.Quantity,
                Code = i.ProductVariant?.Code,
                Size = i.ProductVariant?.Size,
                Price = i.ProductVariant?.Price ?? 0,
                Thumbnail = i.ProductVariant?.Images?.FirstOrDefault()?.ImageUrl,
                ProductId = i.ProductVariant?.ProductId ?? 0,
                ProductName = i.ProductVariant?.Product?.Name,
                ColorName = i.ProductVariant?.Color?.Name,
                ColorHex = i.ProductVariant?.Color?.HexCode,
                Discount = i.ProductVariant?.Discount,
                RealPrice = i.ProductVariant?.RealPrice ?? 0,
                StockQuantity = i.ProductVariant?.StockQuantity ?? 0
            });

            return list;
        }

        public async Task<int> CheckoutAsync(string userId, int addressId)
        {
            var cart = await _unitOfWork.Carts.GetUserCartAsync(userId);
            if (cart == null || !cart.Items.Any()) throw new InvalidOperationException("Cart is empty");

            // create order
            var order = new Order
            {
                UserId = userId,
                AddressId = addressId,
                Status = OrderStatus.Pending,
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
    }
}
