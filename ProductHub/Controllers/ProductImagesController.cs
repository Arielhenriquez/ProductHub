using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductHub.Application.Common.BaseResponse;
using ProductHub.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductHub.Controllers;

[Route("api/products/{productId}/images")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ProductImagesController : ControllerBase
{
    private readonly IProductImageService _productImageService;

    public ProductImagesController(IProductImageService productImageService)
    {
        _productImageService = productImageService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Uploads multiple images to a product (Admin only)")]
    public async Task<IActionResult> UploadImages([FromRoute] Guid productId, [FromForm] List<IFormFile> files, CancellationToken cancellationToken)
    {
        if (files == null || files.Count == 0)
            return BadRequest(BaseResponse.BadRequest("No images provided."));

        var result = await _productImageService.UploadImagesAsync(productId, files, cancellationToken);
        return Ok(BaseResponse.Created(result));
    }

    [HttpDelete("{imageId}")]
    [SwaggerOperation(Summary = "Deletes a product image (soft delete, Admin only)")]
    public async Task<IActionResult> DeleteImage([FromRoute] Guid productId, [FromRoute] Guid imageId, CancellationToken cancellationToken)
    {
        await _productImageService.DeleteImageAsync(productId, imageId, cancellationToken);
        return Ok(BaseResponse.Deleted(imageId));
    }

    [HttpPut("{imageId}/set-main")]
    [SwaggerOperation(Summary = "Sets an image as the main image for the product (Admin only)")]
    public async Task<IActionResult> SetMainImage([FromRoute] Guid productId, [FromRoute] Guid imageId, CancellationToken cancellationToken)
    {
        await _productImageService.SetMainImageAsync(productId, imageId, cancellationToken);
        return Ok(BaseResponse.Updated("Main image set successfully."));
    }
}
