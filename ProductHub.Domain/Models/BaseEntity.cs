namespace ProductHub.Domain.Models;

public interface IBase
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? DeletedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }

}
public class BaseEntity : IBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsDeleted { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? DeletedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
}
