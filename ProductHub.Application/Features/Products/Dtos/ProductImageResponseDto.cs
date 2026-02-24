namespace ProductHub.Application.Features.Products.Dtos;

public class ProductImageResponseDto
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }
}
