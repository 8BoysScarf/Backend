using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Repositry;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;

namespace _8Boys.Services
{
    public class ProductVariantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Cloudinary _cloudinary;
        private readonly ColorService _colorService;

        public ProductVariantService(IUnitOfWork unitOfWork, Cloudinary cloudinary, ColorService colorService)
        {
            _unitOfWork = unitOfWork;
            _cloudinary = cloudinary;
            _colorService = colorService;
        }

        public async Task<ProductVariant?> GetByIdAsync(int id)
        {
            return await _unitOfWork.ProductVariants.Query()
                .Include(v => v.Images)
                .Include(v => v.Color)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<int> AddVariantAsync(AddVariantDTO dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null) throw new InvalidOperationException("Product not found");
            if (_unitOfWork.ProductVariants.Query().Any(v => v.Code == dto.Code))
            {
                throw new InvalidDataException("الكود مسجل مسبقاً");
            }
            int? effectiveColorId = dto.ColorId;
            if (effectiveColorId == null && !string.IsNullOrWhiteSpace(dto.ColorName))
            {
                effectiveColorId = await _colorService.CreateAsync(new DTOs.AddColorDTO { Name = dto.ColorName, HexCode = dto.HexCode });
            }

            // check uniqueness of variant (ProductId, ColorId, Size)
            var duplicate = await _unitOfWork.ProductVariants.Query()
                .AnyAsync(v => v.ProductId == dto.ProductId && v.ColorId == effectiveColorId && v.Size == dto.Size);
            if (duplicate)
                throw new InvalidDataException("يوجد متغير بنفس المنتج واللون والحجم بالفعل");

            var variant = new ProductVariant
            {
                ProductId = dto.ProductId,
                ColorId = effectiveColorId,
                Size = dto.Size,
                RealPrice = dto.RealPrice,
                Price = dto.Price,
                Discount = dto.Discount,
                StockQuantity = dto.StockQuantity,
                Code = dto.Code,
                Images = new List<ProductImage>()
            };

            if (dto.ImageFiles != null)
            {
                var uploadTasks = dto.ImageFiles.Select(f => FileUpload.UploadAsync(f, _cloudinary));
                var results = await Task.WhenAll(uploadTasks);
                foreach (var r in results)
                {
                    variant.Images.Add(new ProductImage { ImageUrl = r.SecureUrl.ToString() });
                }
            }

            await _unitOfWork.ProductVariants.AddAsync(variant);
            await _unitOfWork.SaveChangesAsync();
            return variant.Id;
        }

        public async Task<int> RemoveVariantAsync(int id)
        {
            var v = await _unitOfWork.ProductVariants.Query()
                .Include(v => v.Images)
                .FirstOrDefaultAsync(v => v.Id == id); ;
            if (v == null) return 0;

            // delete images from Cloudinary if any
            if (v.Images != null)
            {
                foreach (var img in v.Images.ToList())
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(img.ImageUrl))
                        {
                            await FileUpload.DeleteImageAsync(img.ImageUrl, _cloudinary);
                        }
                    }
                    catch
                    {
                        // ignore deletion errors, continue
                    }
                }
            }

            _unitOfWork.ProductVariants.Remove(v);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> UpdateVariantAsync(UpdateVariantDTO dto)
        {
            var existing = await _unitOfWork.ProductVariants.GetByIdAsync(dto.Id);
            if (existing == null) throw new InvalidOperationException("Variant not found");

            if (_unitOfWork.ProductVariants.Query().Any(v=>v.Code==dto.Code && v.Id != dto.Id))
            {
                throw new InvalidDataException("الكود مسجل مسبقاً");
            }
            int? effectiveColorId = dto.ColorId;
            if (effectiveColorId == null && !string.IsNullOrWhiteSpace(dto.ColorName))
            {
                effectiveColorId = await _colorService.CreateAsync(new DTOs.AddColorDTO { Name = dto.ColorName, HexCode = dto.HexCode });
            }

            // check uniqueness of variant (ProductId, ColorId, Size) excluding current
            var duplicate = await _unitOfWork.ProductVariants.Query()
                .AnyAsync(v => v.Id != dto.Id && v.ProductId == dto.ProductId && v.ColorId == effectiveColorId && v.Size == dto.Size);
            if (duplicate)
                throw new InvalidDataException("يوجد متغير بنفس المنتج واللون والحجم بالفعل");

            existing.ColorId = effectiveColorId;
            existing.Size = dto.Size;
            existing.RealPrice = dto.RealPrice;
            existing.Price = dto.Price;
            existing.Discount = dto.Discount;
            existing.StockQuantity = dto.StockQuantity;
            existing.Code = dto.Code;

            // handle new image uploads if provided
            if (dto.ImageFiles != null)
            {
                var uploadTasks = dto.ImageFiles.Select(f => FileUpload.UploadAsync(f, _cloudinary));
                var results = await Task.WhenAll(uploadTasks);
                foreach (var r in results)
                {
                    existing.Images.Add(new ProductImage { ImageUrl = r.SecureUrl.ToString() });
                }
            }

            _unitOfWork.ProductVariants.Update(existing);
            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
