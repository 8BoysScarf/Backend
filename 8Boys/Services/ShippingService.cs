using _8Boys.Models;
using _8Boys.Repositry;
using Microsoft.EntityFrameworkCore;

namespace _8Boys.Services
{
    public class ShippingService
    {
        private readonly IUnitOfWork _unitOfWork;

        private const decimal DefaultShipping = 40m;

        public ShippingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Return shipping price (currency) based on user's default address city
        public async Task<decimal> GetShippingPriceForUserAsync(string userId)
        {
            var addr = await _unitOfWork.Addresses.Query().FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
            if (addr == null) return DefaultShipping;

            if (string.IsNullOrWhiteSpace(addr.City)) return DefaultShipping;

            var city = addr.City.Trim();
            var cs = await _unitOfWork.CityShippings.Query().FirstOrDefaultAsync(c => c.City == city);
            if (cs != null)
                return cs.Price;

            return DefaultShipping;
        }

        // Admin: get all city prices
        public async Task<IEnumerable<CityShipping>> GetAllCityPricesAsync()
        {
            return await _unitOfWork.CityShippings.GetAllAsync();
        }

        // Admin: set or add price for a city
        public async Task SetCityPriceAsync(string city, decimal price)
        {
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required", nameof(city));
            var existing = await _unitOfWork.CityShippings.Query().FirstOrDefaultAsync(c => c.City == city);
            if (existing != null)
            {
                existing.Price = price;
                _unitOfWork.CityShippings.Update(existing);
            }
            else
            {
                await _unitOfWork.CityShippings.AddAsync(new CityShipping { City = city.Trim(), Price = price });
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CityShipping?> GetCityAsync(int id)
        {
            return await _unitOfWork.CityShippings.GetByIdAsync(id);
        }

        public async Task DeleteCityAsync(int id)
        {
            var existing = await _unitOfWork.CityShippings.GetByIdAsync(id);
            if (existing == null) return;
            _unitOfWork.CityShippings.Remove(existing);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
