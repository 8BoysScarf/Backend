using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Repositry;
using Microsoft.EntityFrameworkCore;

namespace _8Boys.Services
{
    public class AddressService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AddressDTO>> GetUserAddressesAsync(string userId)
        {
            var list = await _unitOfWork.Addresses.Query()
                .Where(a => a.UserId == userId)
                .Select(a => new AddressDTO { Id = a.Id, City = a.City, Street = a.Street, IsDefault = a.IsDefault })
                .ToListAsync();

            return list;
        }

        public async Task<int> AddAddressAsync(string userId, AddAddressDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            if (dto.IsDefault)
            {
                // unset previous default
                var prev = await _unitOfWork.Addresses.Query().Where(a => a.UserId == userId && a.IsDefault).ToListAsync();
                foreach (var p in prev)
                {
                    p.IsDefault = false;
                    _unitOfWork.Addresses.Update(p);
                }
            }

            var address = new Address
            {
                UserId = userId,
                City = dto.City,
                Street = dto.Street,
                IsDefault = dto.IsDefault
            };

            await _unitOfWork.Addresses.AddAsync(address);
            await _unitOfWork.SaveChangesAsync();
            return address.Id;
        }

        public async Task UpdateAddressAsync(string userId, UpdateAddressDTO dto)
        {
            var existing = await _unitOfWork.Addresses.GetByIdAsync(dto.Id);
            if (existing == null || existing.UserId != userId) throw new InvalidOperationException("Address not found");

            if (dto.IsDefault)
            {
                var prev = await _unitOfWork.Addresses.Query().Where(a => a.UserId == userId && a.IsDefault && a.Id != dto.Id).ToListAsync();
                foreach (var p in prev)
                {
                    p.IsDefault = false;
                    _unitOfWork.Addresses.Update(p);
                }
            }

            existing.City = dto.City;
            existing.Street = dto.Street;
            existing.IsDefault = dto.IsDefault;

            _unitOfWork.Addresses.Update(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeleteAddressAsync(string userId, int id)
        {
            var existing = await _unitOfWork.Addresses.GetByIdAsync(id);
            if (existing == null || existing.UserId != userId) return 0;

            _unitOfWork.Addresses.Remove(existing);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetDefaultAddressAsync(string userId, int id)
        {
            var existing = await _unitOfWork.Addresses.GetByIdAsync(id);
            if (existing == null || existing.UserId != userId) throw new InvalidOperationException("Address not found");

            var prev = await _unitOfWork.Addresses.Query().Where(a => a.UserId == userId && a.IsDefault).ToListAsync();
            foreach (var p in prev)
            {
                p.IsDefault = false;
                _unitOfWork.Addresses.Update(p);
            }

            existing.IsDefault = true;
            _unitOfWork.Addresses.Update(existing);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
