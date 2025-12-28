namespace ProductHub.Application.Features.Categories.Dtos;

public class CategoryResponseDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
