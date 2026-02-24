namespace ProductHub.Domain.Models;

public class Products : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public bool IsActive { get; set; } = true;
    public int QuantityInStock { get; set; }    
    public required Guid CategoryId { get; set; }
    public Categories Category { get; set; } = null!;
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

}
