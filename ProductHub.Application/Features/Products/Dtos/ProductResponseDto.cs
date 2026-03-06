using ProductHub.Application.Features.Categories.Dtos;

namespace ProductHub.Application.Features.Products.Dtos;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public bool IsActive { get; set; }
    public int QuantityInStock { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public List<CategoryResponseDto> CategoryResponses { get; set; } = new();
    public List<ProductImageResponseDto> Images { get; set; } = new();
}
