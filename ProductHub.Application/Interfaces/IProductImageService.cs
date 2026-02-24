using Microsoft.AspNetCore.Http;
using ProductHub.Application.Features.Products.Dtos;

namespace ProductHub.Application.Interfaces;

public interface IProductImageService
{
    Task<List<ProductImageResponseDto>> UploadImagesAsync(Guid productId, List<IFormFile> files, CancellationToken cancellationToken = default);
    Task DeleteImageAsync(Guid productId, Guid imageId, CancellationToken cancellationToken = default);
    Task SetMainImageAsync(Guid productId, Guid imageId, CancellationToken cancellationToken = default);
}
