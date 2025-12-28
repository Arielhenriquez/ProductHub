using ProductHub.Application.Common.Pagination;
using ProductHub.Application.Features.Categories.Dtos;

namespace ProductHub.Application.Interfaces;

public interface ICategoryService : IBaseService<CategoryDto, CategoryResponseDto>
{
    Task<Paged<CategoryResponseDto>> GetPagedCategories(PaginationQuery query, CancellationToken cancellationToken = default);
}
