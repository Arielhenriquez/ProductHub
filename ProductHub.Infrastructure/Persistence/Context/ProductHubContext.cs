using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProductHub.Domain.Models;
using ProductHub.Infrastructure.Persistence.Configurations;

namespace ProductHub.Infrastructure.Persistence.Context;

public interface IProductHubContext : IDbContext { }
public class ProductHubContext(DbContextOptions options, IHttpContextAccessor context) : BaseDbContext(options, context), IProductHubContext
{
    public DbSet<Categories>? Categories { get; set; }
    public DbSet<Products>? Products { get; set; }
    public DbSet<Users> Users { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductsConfiguration());
        modelBuilder.ApplyConfiguration(new ProductImageConfiguration());
       
        base.OnModelCreating(modelBuilder);
    }
}