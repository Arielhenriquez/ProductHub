using System.Text.Json.Serialization;

namespace ProductHub.Application.Features.Categories.Dtos;

public class CategoryDto
{
    [JsonIgnore]
    public Guid Id { get; set; } 
    public required string Name { get; set; }
    public required string Description { get; set; }
}
