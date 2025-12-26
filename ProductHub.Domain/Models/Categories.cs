namespace ProductHub.Domain.Models;

public class Categories : BaseEntity
{
    public required string Name { get; set; }
    public string? Description {  get; set; }
    public ICollection<Products> Products { get; set; } = new List<Products>();

}
