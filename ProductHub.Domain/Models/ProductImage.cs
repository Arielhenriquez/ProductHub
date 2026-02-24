using System.ComponentModel.DataAnnotations;

namespace ProductHub.Domain.Models;

public class ProductImage : BaseEntity
{
    public required Guid ProductId { get; set; }
    
    [Required]
    [MaxLength(500)]
    public required string Url { get; set; }
    
    public string? FileName { get; set; }
    
    public int SortOrder { get; set; } = 0;
    
    public bool IsMain { get; set; } = false;

    public Products Product { get; set; } = null!;
}
