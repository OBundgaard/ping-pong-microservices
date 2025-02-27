using Microsoft.AspNetCore.Mvc;

namespace PongService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PongController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Pong()
    {
        await Task.Delay(100);
        return Ok();
    }
}
