using ProductHub.Application.Common.Pagination;
using ProductHub.Application.Features.Products.Dtos;

namespace ProductHub.Application.Interfaces;

public interface IProductService : IBaseService<CreateProductDto, ProductResponseDto>
{
    Task<Paged<ProductResponseDto>> GetPagedProducts(PaginationQuery query, CancellationToken cancellationToken = default);
}
