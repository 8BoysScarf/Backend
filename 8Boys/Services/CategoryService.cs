using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Repositry;
using Microsoft.EntityFrameworkCore;

namespace _8Boys.Services
{
    public class CategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var all = await _unitOfWork.Categories.Query()
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentName = c.ParentCategory != null
                        ? c.ParentCategory.Name
                        : null
                })
                .ToListAsync();

            return all;
        }
        public async Task<int> CreateAsync(AddCategoryDTO dto)
        {
            // validation
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("اسم التصنيف مطلوب", nameof(dto.Name));

            if (dto.ParentCategoryId.HasValue)
            {
                if (dto.ParentCategoryId.Value <= 0)
                    throw new ArgumentException("التصنيف الاساسي يجب ان يكون موجود", nameof(dto.ParentCategoryId));

                var parent = await _unitOfWork.Categories.GetByIdAsync(dto.ParentCategoryId.Value);
                if (parent == null)
                    throw new InvalidOperationException("التصنيف الاساسي يجب ان يكون موجود");
            }

           
            var exists = await _unitOfWork.Categories.Query()
                .AnyAsync(c => c.Name == dto.Name && c.ParentCategoryId == dto.ParentCategoryId);

            if (exists)
                throw new InvalidOperationException("التصنيف موجود من قبل");

            var category = new Category
            {
                Name = dto.Name.Trim(),
                ParentCategoryId = dto.ParentCategoryId
            };

            await _unitOfWork.Categories.AddAsync(category);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(UpdateCategoryDTO category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            if (category.Id <= 0) throw new ArgumentException("التصنيف لم يعد موجود", nameof(category.Id));
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("اسم التصنيف مطلوب", nameof(category.Name));

            var existing = await _unitOfWork.Categories.GetByIdAsync(category.Id);
            if (existing == null) throw new InvalidOperationException("التصنيف لم يعد موجود");

            if (category.ParentCategoryId.HasValue)
            {
                if (category.ParentCategoryId == category.Id)
                    throw new InvalidOperationException("التصنيف لا يمكن ان يكون التصنيف الاساسي لنفسه");

                var parent = await _unitOfWork.Categories.GetByIdAsync(category.ParentCategoryId.Value);
                if (parent == null)
                    throw new InvalidOperationException("التصنيف الاساسي يجب ان يكون موجود");
            }

            
            var duplicate = await _unitOfWork.Categories.Query()
                .AnyAsync(c => c.Id != category.Id && c.Name == category.Name && c.ParentCategoryId == category.ParentCategoryId);

            if (duplicate)
                throw new InvalidOperationException("التصنيف موجود من قبل");

           
            existing.Name = category.Name.Trim();
            existing.ParentCategoryId = category.ParentCategoryId;

            _unitOfWork.Categories.Update(existing);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return 0;
            _unitOfWork.Categories.Remove(category);
            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
