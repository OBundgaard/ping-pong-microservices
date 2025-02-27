using Microsoft.AspNetCore.Mvc;

namespace Core.Interfaces.Controllers;

public interface IControllerAll<T> : IController<T>
{
    public Task<ActionResult<IEnumerable<T>>> GetAllAsync();
}
