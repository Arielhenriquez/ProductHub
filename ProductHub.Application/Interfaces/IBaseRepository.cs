using ProductHub.Domain.Models;

namespace ProductHub.Application.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : class, IBase
{
    IQueryable<TEntity> Query();
    IQueryable<TEntity> ListOrderedBy();
    Task<IEnumerable<TEntity>> GetAll();
    Task<TEntity> GetById(Guid id, CancellationToken cancellationToken);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<ICollection<TEntity>> AddRange(ICollection<TEntity> entities, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<ICollection<TEntity>> UpdateRange(ICollection<TEntity> entities, CancellationToken cancellationToken = default);
    Task PatchAsync(Guid id, Dictionary<string, object> updates, CancellationToken cancellationToken = default);
    Task<TEntity> Delete(Guid id, CancellationToken cancellationToken);
    Task RemoveRange(ICollection<TEntity> entities, CancellationToken cancellationToken = default);
    Task SaveChanges(CancellationToken cancellationToken);
}
