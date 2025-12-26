using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProductHub.Domain.Models;
using ProductHub.Infrastructure.Extensions;
namespace ProductHub.Infrastructure.Persistence.Context;


public interface IDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

}
public abstract class BaseDbContext : DbContext, IDbContext
{
    protected readonly IHttpContextAccessor _context;
    protected BaseDbContext(DbContextOptions options, IHttpContextAccessor context) : base(options)
    {
        _context = context;
    }

    private void SetAuditEntities()
    {
        string email = _context?.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Upn)?.Value ?? "System";
        foreach (var entry in ChangeTracker.Entries<IBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:

                    entry.Entity.IsDeleted = false;
                    entry.Entity.CreatedDate = DateTimeOffset.UtcNow;
                    entry.Entity.CreatedBy = email;
                    break;

                case EntityState.Modified:
                    entry.Property(x => x.CreatedDate).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;
                    entry.Entity.UpdatedDate = DateTimeOffset.UtcNow;
                    entry.Entity.UpdatedBy = email;
                    break;

                case EntityState.Deleted:
                    entry.Property(x => x.CreatedDate).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedDate = DateTimeOffset.UtcNow;
                    entry.Entity.DeletedBy = email;
                    break;

                default:
                    break;
            }
        }
    }

    public override int SaveChanges()
    {
        SetAuditEntities();
        return base.SaveChanges();
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetAuditEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
        foreach (var type in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IBase).IsAssignableFrom(type.ClrType))
            {
                modelBuilder.SetSoftDeleteFilter(type.ClrType);
            }
        }
    }
}

