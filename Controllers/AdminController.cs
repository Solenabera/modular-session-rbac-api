using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecureSessionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Optimization & Security: Enforces strict RBAC rejection parameters early
public class AdminController : ControllerBase
{
    [HttpGet("dashboard")]
    public IActionResult GetAdminMetrics()
    {
        return Ok(new {
            message = "Welcome to the Master Administration control interface.",
            serverTime = DateTime.UtcNow,
            systemHealth = "Optimal"
        });
    }
}