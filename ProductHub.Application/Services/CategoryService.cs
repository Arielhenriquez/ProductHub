using Microsoft.EntityFrameworkCore;
using ProductHub.Application.Common;
using ProductHub.Application.Common.Extensions;
using ProductHub.Application.Common.Pagination;
using ProductHub.Application.Features.Categories.Dtos;
using ProductHub.Application.Features.Categories.Predicates;
using ProductHub.Application.Features.Categories.Projections;
using ProductHub.Application.Interfaces;
using ProductHub.Domain.Models;

namespace ProductHub.Application.Services;

public class CategoryService : BaseService<Categories, CategoryDto, CategoryResponseDto>, ICategoryService
{
    public CategoryService(IBaseRepository<Categories> repository) : base(repository)
    {
    }

    public Task<Paged<CategoryResponseDto>> GetPagedCategories(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        return _repository.Query()
       .AsNoTracking()
       .Where(CategoriesPredicates.Search(query.Search))
       .OrderByDescending(p => p.CreatedDate)
       .Select(CategoriesProjections.Search)
       .Paginate(query.PageSize, query.PageNumber, cancellationToken);
    }

    protected override CategoryResponseDto MapToDto(Categories entity)
    {
        return new CategoryResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };
    }

    protected override Categories MapToEntity(CategoryDto dto)
    {
        return new Categories
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description
        };
    }

    protected override void UpdateEntity(Categories entity, CategoryDto dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
    }
}
