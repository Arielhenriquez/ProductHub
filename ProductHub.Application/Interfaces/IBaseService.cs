namespace ProductHub.Application.Interfaces;

public interface IBaseService<TRequest, TResponse> where TResponse : class
{
    //Task<Paged<TDto>> GetPagedAsync(PaginationQuery paginationQuery, CancellationToken cancellationToken);
    Task<IEnumerable<TResponse>> GetAllAsync();
    Task<TResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TResponse> AddAsync(TRequest dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, TRequest dto, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

