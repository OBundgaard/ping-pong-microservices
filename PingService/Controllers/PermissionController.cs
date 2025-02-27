using Core.Helpers;
using Core.Interfaces.Controllers;
using Core.Interfaces.Repositories;
using Core.Models.PingService;
using Microsoft.AspNetCore.Mvc;

namespace PingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionController(IRepository<Permissions> repository) : Controller, IController<Permissions>
{
    [HttpPost("post")]
    public async Task<ActionResult<Permissions>> PostAsync([FromBody] Permissions entry)
    {
        using (var activity = MonitorService.ActivitySource.StartActivity("[Permission Controller @ Ping Service] : POST"))
        {
            // Verify entry
            if (entry == null)
            {
                MonitorService.Log.Error("[Permission Controller @ Ping Service] : POST operation FAILED, no object was passed.");
                return BadRequest(); // Return 400
            }

            // Post and verify existence
            var permissions = await repository.PostAsync(entry);

            // Return 201
            return Created(Url.Action(nameof(GetAsync), new { id = permissions.ID }), permissions);
        }
    }

    [HttpGet("get/{id}")]
    public async Task<ActionResult<Permissions?>> GetAsync(int id)
    {
        using (var activity = MonitorService.ActivitySource.StartActivity("[Permission Controller @ Ping Service] : GET"))
        {
            // Get and verify existence
            var permissions = await repository.GetAsync(id);
            if (permissions == null)
            {
                MonitorService.Log.Warning($"[Permission Controller @ Ping Service] : GET operation FAILED, no object with ID: {id} was found.");
                return NotFound(); // Return 404
            }

            // Return 200
            return Ok(permissions);
        }
    }

    [HttpPut("put/{id}")]
    public async Task<ActionResult<Permissions>> PutAsync(int id, [FromBody] Permissions entry)
    {
        using (var activity = MonitorService.ActivitySource.StartActivity("[Permission Controller @ Ping Service] : PUT"))
        {
            // Verify entry
            if (entry == null)
            {
                MonitorService.Log.Error("[Permission Controller @ Ping Service] : PUT operation FAILED, no object was passed.");
                return BadRequest(); // Return 400
            }

            // Verify id
            if (id != entry.ID)
            {
                MonitorService.Log.Error("[Permission Controller @ Ping Service] : PUT operation FAILED, object ID does not match passed ID.");
                return BadRequest(); // Return 400
            }

            // Get and verify existence
            var permissions = await repository.GetAsync(id);
            if (permissions == null)
            {
                MonitorService.Log.Warning($"[Permission Controller @ Ping Service] : PUT operation FAILED, no object with ID: {id} was found.");
                return NotFound(); // Return 404
            }

            // Put
            permissions = await repository.PutAsync(entry);

            // Return 200
            return Ok(permissions);
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        using (var activity = MonitorService.ActivitySource.StartActivity("[Permission Controller @ Ping Service] : DELETE"))
        {
            // Get and verify existence
            var permissions = await repository.GetAsync(id);
            if (permissions == null)
            {
                MonitorService.Log.Warning($"[Permission Controller @ Ping Service] : DELETE operation FAILED, no object with ID: {id} was found.");
                return NotFound(); // Return 404
            }

            // Delete
            await repository.DeleteAsync(id);

            // Return 204
            return NoContent();
        }
    }
}
