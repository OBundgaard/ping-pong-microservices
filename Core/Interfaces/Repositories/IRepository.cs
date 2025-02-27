namespace Core.Interfaces.Repositories;

public interface IRepository<T>
{
    public Task<T> PostAsync(T entity);
    public Task<T?> GetAsync(int id);
    public Task<T> PutAsync(T entity);
    public Task DeleteAsync(int id);
}
