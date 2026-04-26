using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Repositry;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;

namespace _8Boys.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Cloudinary _cloudinary;
        private readonly ColorService _colorService;

        public ProductService(
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            Cloudinary cloudinary,
            ColorService colorService)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _cloudinary = cloudinary;
            _colorService = colorService;
        }

        // =====================
        // Get All (return card DTOs)
        // =====================
        public async Task<IEnumerable<ProductCardDTO>> GetAllAsync()
        {
            var query = _unitOfWork.Products.Query()
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Images)
                .Include(p => p.ProductBadges)
                    .ThenInclude(pb => pb.Badge)
                .AsQueryable();

            var items = await query
                .Select(p => new ProductCardDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category.Name,
                    Price = p.Variants.OrderBy(v => v.Price).Select(v => v.Price).FirstOrDefault(),
                    ThumbnailUrl = p.Variants.SelectMany(v => v.Images).Select(i => i.ImageUrl).FirstOrDefault(),
                    Badges = p.ProductBadges.Select(pb => pb.Badge.Name),
                    StockQuantity = p.Variants.OrderBy(v => v.Price).Select(v => v.StockQuantity).FirstOrDefault(),
                    Code = p.Variants.OrderBy(v => v.Price).Select(v => v.Code).FirstOrDefault(),
                    Discount = p.Variants.OrderBy(v => v.Price).Select(v => v.Discount).FirstOrDefault(),
                    Size = p.Variants.OrderBy(v => v.Price).Select(v => v.Size).FirstOrDefault(),
                    HexCode = p.Variants.OrderBy(v => v.Price).Select(v => v.Color != null ? v.Color.HexCode : null).FirstOrDefault()

                })
                .ToListAsync();

            return items;
        }

        
        
        // =====================
        // Create Product (with initial variant)
        // =====================
        public async Task<int> CreateAsync(AddProductDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Product name is required.", nameof(dto.Name));
            if (dto.CategoryId <= 0) throw new ArgumentException("CategoryId is required.", nameof(dto.CategoryId));


            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            if (category == null) throw new InvalidOperationException("Category not found.");


            if (!string.IsNullOrWhiteSpace(dto.Code))
            {

                var exists = await _unitOfWork.Products.Query()
                    .AnyAsync(p => p.Variants.Any(v => v.Code == dto.Code));
                if (exists) throw new InvalidOperationException("A product variant with the same code already exists.");
            }

            int? effectiveColorId = dto.ColorId;
            if (effectiveColorId == null && !string.IsNullOrWhiteSpace(dto.ColorName))
            {
                effectiveColorId = await _colorService.CreateAsync(new AddColorDTO 
                { 
                    Name = dto.ColorName, 
                    HexCode = dto.HexCode 
                });
            }

            var product = new Product
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                CategoryId = dto.CategoryId,
                Variants = new List<ProductVariant>(),
                ProductBadges = new List<ProductBadge>()
            };

            if (dto.BadgeId.HasValue)
            {
                product.ProductBadges.Add(new ProductBadge
                {
                    BadgeId = dto.BadgeId.Value,
                    Product = product
                });
            }

            // create initial variant from DTO
            var variant = new ProductVariant
            {
                Product = product,
                ColorId = effectiveColorId,
                Size = dto.Size,
                RealPrice = dto.RealPrice,
                Price = dto.Price,
                Discount = dto.Discount,
                StockQuantity = dto.StockQuantity,
                Code = dto.Code,
                Images = new List<ProductImage>()
            };

            // upload images if provided
            if (dto.ImageFiles != null)
            {
                var uploadTasks = dto.ImageFiles.Select(file => FileUpload.UploadAsync(file, _cloudinary));
                var results = await Task.WhenAll(uploadTasks);


                foreach (var result in results)
                {
                    variant.Images.Add(new ProductImage
                    {
                        ImageUrl = result.Url.ToString(),
                        ProductVariantId = variant.Id,
                    });
                }
            }

            product.Variants.Add(variant);

            await _unitOfWork.Products.AddAsync(product);
            return await _unitOfWork.SaveChangesAsync();
        }

        // =====================
        // Update Product
        // =====================
        public async Task<int> UpdateAsync(int id ,UpdateProductDTO product)
        {
            var existing = await _unitOfWork.Products.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException("المنتج لم يعد موجود");

            existing.Name = product.Name.Trim();
            existing.Description = product.Description?.Trim();
            existing.CategoryId = product.CategoryId;


            _unitOfWork.Products.Update(existing);
            return await _unitOfWork.SaveChangesAsync();
        }

        // =====================
        // Delete Product
        // =====================
        public async Task<int> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
                return 0;

            _unitOfWork.Products.Remove(product);
            return await _unitOfWork.SaveChangesAsync();
        }

        // =====================
        // New: paged search with filters
        // =====================
        public async Task<PagedResult<ProductCardDTO>> SearchPagedAsync(
            int page = 1,
            int pageSize = 10,
            string? name = null,
            int? categoryId = null,
            int? colorId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? badgeId = null)
        {
            var query = _unitOfWork.Products.Query()
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Images)
                .Include(p => p.ProductBadges)
                    .ThenInclude(pb => pb.Badge)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(p => p.Name.Contains(name));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (colorId.HasValue)
                query = query.Where(p => p.Variants.Any(v => v.ColorId == colorId.Value));

            if (minPrice.HasValue)
                query = query.Where(p => p.Variants.Any(v => v.Price >= minPrice.Value));

            if (maxPrice.HasValue)
                query = query.Where(p => p.Variants.Any(v => v.Price <= maxPrice.Value));

            if (badgeId.HasValue)
                query = query.Where(p => p.ProductBadges.Any(pb => pb.BadgeId == badgeId.Value));

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductCardDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Variants.OrderBy(v => v.Price).Select(v => v.Code).FirstOrDefault(),
                    CategoryName = p.Category.Name,
                    Price = p.Variants.OrderBy(v => v.Price).Select(v => v.Price).FirstOrDefault(),
                    ThumbnailUrl = p.Variants.SelectMany(v => v.Images).Select(i => i.ImageUrl).FirstOrDefault(),
                    Badges = p.ProductBadges.Select(pb => pb.Badge.Name),
                    StockQuantity = p.Variants.OrderBy(v => v.Price).Select(v => v.StockQuantity).FirstOrDefault()
                })
                .ToListAsync();

            return new PagedResult<ProductCardDTO>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        // =====================
        // New: Get Product Details as DTO
        // =====================
        public async Task<ProductDetailsDTO?> GetDetailsDtoAsync(int id)
        {
            var p = await _productRepository.GetWithDetailsAsync(id);
            if (p == null) return null;

            return new ProductDetailsDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                Badges = p.ProductBadges.Select(pb => pb.Badge.Name),
                Variants = p.Variants.Select(v => new ProductVariantDTO
                {
                    Id = v.Id,
                    ColorId = v.ColorId,
                    ColorName = v.Color != null ? v.Color.Name : null,
                    ColorHex = v.Color != null ? v.Color.HexCode : null,
                    Size = v.Size,
                    RealPrice = v.RealPrice,
                    Price = v.Price,
                    Discount = v.Discount,
                    StockQuantity = v.StockQuantity,
                    Code = v.Code,
                    ImageUrls = v.Images.Select(i => i.ImageUrl)
                })
            };
        }
    }
}