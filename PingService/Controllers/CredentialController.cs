using Core.Helpers;
using Core.Interfaces.Controllers;
using Core.Interfaces.Repositories;
using Core.Models.PingService;
using Microsoft.AspNetCore.Mvc;

namespace PingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CredentialController(IRepositoryAll<Credentials> repository) : Controller, IControllerAll<Credentials>
{
    [HttpPost("post")]
    public async Task<ActionResult<Credentials>> PostAsync([FromBody] Credentials entry)
    {
        using (var activity = MonitorService.ActivitySource.StartActivity("[Credential Controller @ Ping Service] : POST"))
        {
            // Verify entry
            if (entry == null)
            {
                MonitorService.Log.Error("[Credential Controller @ Ping Service] : POST operation FAILED, no object was passed.");
                return BadRequest(); // Return 400
            }

            // Post and verify existence
            var credentials = await repository.PostAsync(entry);

            // Return 201
            return Created(Url.Action(nameof(GetAsync), new { id = credentials.ID }), credentials);
        }
    }

    [HttpGet("get/{id}")]
    public async Task<ActionResult<Credentials?>> GetAsync(int id)
    {
        using (var activity = MonitorService.ActivitySource.StartActivity("[Credential Controller @ Ping Service] : GET"))
        {
            // Get and verify existence
            var credentials = await repository.GetAsync(id);
            if (credentials == null)
            {
                MonitorService.Log.Warning($"[Credential Controller @ Ping Service] : GET operation FAILED, no object with ID: {id} was found.");
                return NotFound(); // Return 404
            }

            // Return 200
            return Ok(credentials);
        }
    }

    [HttpGet("getall")]
    public async Task<ActionResult<IEnumerable<Credentials>>> GetAllAsync()
    {
        using (var activity = MonitorService.ActivitySource.StartActivity("[Credential Controller @ Ping Service] : GET ALL"))
        {
            // Get all
            var credentials = await repository.GetAllAsync();

            // Return 200
            return Ok(credentials);
        }
    }

    [HttpPut("put/{id}")]
    public async Task<ActionResult<Credentials>> PutAsync(int id, [FromBody] Credentials entry)
    {
        using (var activity = MonitorService.ActivitySource.StartActivity("[Credential Controller @ Ping Service] : PUT"))
        {
            // Verify entry
            if (entry == null)
            {
                MonitorService.Log.Error("[Credential Controller @ Ping Service] : PUT operation FAILED, no object was passed.");
                return BadRequest(); // Return 400
            }

            // Verify id
            if (id != entry.ID)
            {
                MonitorService.Log.Error("[Credential Controller @ Ping Service] : PUT operation FAILED, object ID does not match passed ID.");
                return BadRequest(); // Return 400
            }

            // Get and verify existence
            var credentials = await repository.GetAsync(id);
            if (credentials == null)
            {
                MonitorService.Log.Warning($"[Credential Controller @ Ping Service] : PUT operation FAILED, no object with ID: {id} was found.");
                return NotFound(); // Return 404
            }

            // Put
            credentials = await repository.PutAsync(entry);

            // Return 200
            return Ok(credentials);
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        using (var activity = MonitorService.ActivitySource.StartActivity("[Credential Controller @ Ping Service] : DELETE"))
        {
            // Get and verify existence
            var credentials = await repository.GetAsync(id);
            if (credentials == null)
            {
                MonitorService.Log.Warning($"[Credential Controller @ Ping Service] : DELETE operation FAILED, no object with ID: {id} was found.");
                return NotFound(); // Return 404
            }

            // Delete
            await repository.DeleteAsync(id);

            // Return 204
            return NoContent();
        }
    }
}
