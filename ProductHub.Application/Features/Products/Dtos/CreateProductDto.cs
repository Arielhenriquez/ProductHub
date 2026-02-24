using System.Text.Json.Serialization;

namespace ProductHub.Application.Features.Products.Dtos;

public class CreateProductDto
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public bool IsActive { get; set; } = true;
    public int QuantityInStock { get; set; }
    public required Guid CategoryId { get; set; }
}
