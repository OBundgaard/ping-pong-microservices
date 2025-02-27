using Microsoft.AspNetCore.Mvc;

namespace Core.Interfaces.Controllers;

public interface IController<T>
{
    public Task<ActionResult<T>> PostAsync(T entry);
    public Task<ActionResult<T?>> GetAsync(int id);
    public Task<ActionResult<T>> PutAsync(int id, T entry);
    public Task<ActionResult> DeleteAsync(int id);
}
