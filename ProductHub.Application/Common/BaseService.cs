using ProductHub.Application.Interfaces;
using ProductHub.Domain.Models;

namespace ProductHub.Application.Common;
public class BaseService<TEntity, TRequest, TResponse> : IBaseService<TRequest, TResponse>
    where TEntity : class, IBase
    where TRequest : class
    where TResponse : class
{
    protected readonly IBaseRepository<TEntity> _repository;

    public BaseService(IBaseRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public virtual async Task<IEnumerable<TResponse>> GetAllAsync()
    {
        var entities = await _repository.GetAll();
        return entities.Select(MapToDto);
    }

    public virtual async Task<TResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetById(id, cancellationToken);
        return MapToDto(entity);
    }

    public virtual async Task<TResponse> AddAsync(TRequest dto, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(dto);
        var addedEntity = await _repository.AddAsync(entity, cancellationToken);
        return MapToDto(addedEntity);
    }

    public virtual async Task UpdateAsync(Guid id, TRequest dto, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetById(id, cancellationToken) ?? throw new KeyNotFoundException($"Entity with id {id} not found");
        UpdateEntity(entity, dto);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _repository.Delete(id, cancellationToken);
    }
    protected virtual TResponse MapToDto(TEntity entity)
    {
        throw new NotImplementedException("Implementa el mapeo de entidad a DTO.");
    }

    protected virtual TEntity MapToEntity(TRequest dto)
    {
        throw new NotImplementedException("Implementa el mapeo de DTO a entidad.");
    }

    protected virtual void UpdateEntity(TEntity entity, TRequest dto)
    {
        throw new NotImplementedException("Implementa la actualización de entidad en base a DTO.");
    }
}