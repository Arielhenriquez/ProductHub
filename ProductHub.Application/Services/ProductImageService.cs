using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProductHub.Application.Common.Exceptions;
using ProductHub.Application.Features.Products.Dtos;
using ProductHub.Application.Interfaces;
using ProductHub.Domain.Models;

namespace ProductHub.Application.Services;

public class ProductImageService : IProductImageService
{
    private readonly IBaseRepository<ProductImage> _imageRepository;
    private readonly IBaseRepository<Products> _productRepository;
    private readonly IBlobService _blobService;

    public ProductImageService(
        IBaseRepository<ProductImage> imageRepository,
        IBaseRepository<Products> productRepository,
        IBlobService blobService)
    {
        _imageRepository = imageRepository;
        _productRepository = productRepository;
        _blobService = blobService;
    }

    public async Task<List<ProductImageResponseDto>> UploadImagesAsync(Guid productId, List<IFormFile> files, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.Query()
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product == null)
            throw new NotFoundException(nameof(Products), productId);

        var uploadedImages = new List<ProductImageResponseDto>();
        bool hasMainImage = product.Images.Any(i => i.IsMain && !i.IsDeleted);
        int currentMaxSortOrder = product.Images.Where(i => !i.IsDeleted).Select(i => i.SortOrder).DefaultIfEmpty(0).Max();

        var newImages = new List<ProductImage>();

        foreach (var file in files)
        {
            var url = await _blobService.UploadImageAsync(file, cancellationToken);

            var image = new ProductImage
            {
                ProductId = productId,
                Url = url,
                FileName = file.FileName,
                SortOrder = ++currentMaxSortOrder,
                IsMain = !hasMainImage
            };

            if (!hasMainImage)
            {
                hasMainImage = true;
            }

            newImages.Add(image);
            uploadedImages.Add(new ProductImageResponseDto
            {
                Id = image.Id,
                Url = image.Url,
                IsMain = image.IsMain,
                SortOrder = image.SortOrder // Note: Id will be empty here until saved if we rely on DB gen, but let's assume AddRange saves.
            });
        }

        // BaseRepository AddRange calls SaveChanges
        await _imageRepository.AddRange(newImages, cancellationToken);

        // Update IDs in response after save
        for (int i = 0; i < newImages.Count; i++)
        {
            uploadedImages[i].Id = newImages[i].Id;
        }

        return uploadedImages;
    }

    public async Task DeleteImageAsync(Guid productId, Guid imageId, CancellationToken cancellationToken = default)
    {
        var image = await _imageRepository.Query()
            .FirstOrDefaultAsync(i => i.Id == imageId && i.ProductId == productId, cancellationToken);

        if (image == null)
            throw new NotFoundException(nameof(ProductImage), imageId);

        // Soft delete using Update
        image.IsDeleted = true;
        image.DeletedDate = DateTimeOffset.UtcNow;

        await _imageRepository.UpdateAsync(image, cancellationToken);
    }

    public async Task SetMainImageAsync(Guid productId, Guid imageId, CancellationToken cancellationToken = default)
    {
        var productImages = await _imageRepository.Query()
            .Where(i => i.ProductId == productId && !i.IsDeleted)
            .ToListAsync(cancellationToken);

        var targetImage = productImages.FirstOrDefault(i => i.Id == imageId);
        if (targetImage == null)
            throw new NotFoundException(nameof(ProductImage), imageId);

        foreach (var img in productImages)
        {
            img.IsMain = (img.Id == imageId);
        }

        await _imageRepository.UpdateRange(productImages, cancellationToken);
    }
}
