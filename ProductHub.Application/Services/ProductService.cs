using Microsoft.EntityFrameworkCore;
using ProductHub.Application.Common;
using ProductHub.Application.Common.Exceptions;
using ProductHub.Application.Common.Extensions;
using ProductHub.Application.Common.Pagination;
using ProductHub.Application.Features.Categories.Dtos;
using ProductHub.Application.Features.Products.Dtos;
using ProductHub.Application.Features.Products.Predicates;
using ProductHub.Application.Features.Products.Projections;
using ProductHub.Application.Interfaces;
using ProductHub.Domain.Models;

namespace ProductHub.Application.Services;

public class ProductService : BaseService<Products, CreateProductDto, ProductResponseDto>, IProductService
{
    public ProductService(IBaseRepository<Products> repository) : base(repository)
    {
    }

    public Task<Paged<ProductResponseDto>> GetPagedProducts(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        return _repository.Query()
            .AsNoTracking()
            .Where(ProductsPredicates.Search(query.Search))
            .OrderByDescending(product => product.CreatedDate)
            .Select(ProductsProjections.Search)
            .Paginate(query.PageSize, query.PageNumber, cancellationToken);
    }

    public override async Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.Query()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (entity is null) throw new NotFoundException(nameof(Products), id);

        return MapToDto(entity);
    }

    protected override ProductResponseDto MapToDto(Products entity)
    {
        return new ProductResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            IsActive = entity.IsActive,
            QuantityInStock = entity.QuantityInStock,
            CategoryResponses = entity.Category == null
                ? new List<CategoryResponseDto>()
                : new List<CategoryResponseDto>
                {
                    new CategoryResponseDto
                    {
                        Id = entity.Category.Id,
                        Name = entity.Category.Name,
                        Description = entity.Category.Description
                    }
                },
            Images = entity.Images?.Where(i => !i.IsDeleted)
                .OrderByDescending(i => i.IsMain)
                .ThenBy(i => i.SortOrder)
                .Select(i => new ProductImageResponseDto
                {
                    Id = i.Id,
                    Url = i.Url,
                    IsMain = i.IsMain,
                    SortOrder = i.SortOrder
                }).ToList() ?? new List<ProductImageResponseDto>()
        };
    }

    protected override Products MapToEntity(CreateProductDto dto)
    {
        return new Products
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            IsActive = dto.IsActive,
            QuantityInStock = dto.QuantityInStock,
            CategoryId = dto.CategoryId
        };
    }

    protected override void UpdateEntity(Products entity, CreateProductDto dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Price = dto.Price;
        entity.IsActive = dto.IsActive;
        entity.QuantityInStock = dto.QuantityInStock;
        entity.CategoryId = dto.CategoryId;
    }
}
