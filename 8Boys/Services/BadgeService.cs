using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Repositry;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace _8Boys.Services
{
    public class BadgeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BadgeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BadgeCardDTO>> GetAllAsync()
        {
            var items = await _unitOfWork.Badges.Query()
                .Select(b => new BadgeCardDTO
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToListAsync();

            return items;
        }

        public async Task<int> CreateAsync(AddBadgeDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name required", nameof(dto.Name));

            var exists = await _unitOfWork.Badges.Query().AnyAsync(b => b.Name == dto.Name);
            if (exists) throw new InvalidOperationException("Badge with same name already exists.");

            var badge = new Badge { Name = dto.Name.Trim() };
            await _unitOfWork.Badges.AddAsync(badge);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(UpdateBadgeDTO dto)
        {
            var existing = await _unitOfWork.Badges.GetByIdAsync(dto.Id);
            if (existing == null) throw new InvalidOperationException("Badge not found");

            existing.Name = dto.Name.Trim();

            _unitOfWork.Badges.Update(existing);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Badges.GetByIdAsync(id);
            if (existing == null) return 0;

            _unitOfWork.Badges.Remove(existing);
            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
