using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Repositry;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace _8Boys.Services
{
    public class ColorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ColorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ColorCardDTO>> GetAllAsync()
        {
            var items = await _unitOfWork.Colors.Query()
                .Select(c => new ColorCardDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    HexCode = c.HexCode
                })
                .ToListAsync();

            return items;
        }

        public async Task<int> CreateAsync(AddColorDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("الاسم موجود من قبل", nameof(dto.Name));

            
            if (!string.IsNullOrWhiteSpace(dto.HexCode))
            {
                var existingByHex = await _unitOfWork.Colors.Query().FirstOrDefaultAsync(c => c.HexCode == dto.HexCode);
                if (existingByHex != null) return existingByHex.Id;
            }

            var color = new Color { Name = dto.Name.Trim(), HexCode = dto.HexCode };
            await _unitOfWork.Colors.AddAsync(color);
            await _unitOfWork.SaveChangesAsync();
            return color.Id;
        }

        public async Task<int> UpdateAsync(UpdateColorDTO dto)
        {
            var existing = await _unitOfWork.Colors.GetByIdAsync(dto.Id);
            if (existing == null) throw new InvalidOperationException("Color not found");

            existing.Name = dto.Name.Trim();
            existing.HexCode = dto.HexCode;

            _unitOfWork.Colors.Update(existing);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Colors.GetByIdAsync(id);
            if (existing == null) return 0;

            _unitOfWork.Colors.Remove(existing);
            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
