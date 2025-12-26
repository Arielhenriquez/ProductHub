using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProductHub.Domain.Models;

namespace ProductHub.Infrastructure.Persistence.Context;

public interface IProductHubContext : IDbContext { }
public class ProductHubContext(DbContextOptions options, IHttpContextAccessor context) : BaseDbContext(options, context), IProductHubContext
{
    public DbSet<Categories>? Categories { get; set; }
    public DbSet<Products>? Products { get; set; }
    public DbSet<Users> Users { get; set; }
}
