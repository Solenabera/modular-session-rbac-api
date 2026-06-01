using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecureSessionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires authentication, but permits any role alignment
public class ResourceController : ControllerBase
{
    [HttpGet("shared-feed")]
    public IActionResult GetSharedData()
    {
        return Ok(new { content = "This standard application content is visible to authenticated users." });
    }
}