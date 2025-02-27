namespace Core.Interfaces.Repositories;

public interface IRepositoryAll<T> : IRepository<T>
{
    public Task<IEnumerable<T>> GetAllAsync();
}
