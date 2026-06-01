using System.Security.Claims;
using SecureSessionApi.Models.Dtos;
using SecureSessionApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace SecureSessionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] UserRegisterDto dto)
    {
        var success = await _authService.RegisterAsync(dto);
        if (!success) return BadRequest(new { message = "Email is already registered." });
        return Ok(new { message = "User registered successfully!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var userSession = await _authService.ValidateUserCredentialsAsync(dto);
        if (userSession == null) return BadRequest(new { message = "Invalid email or password." });

        // Build core security claims tracking identities inside the session cookie
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userSession.Id.ToString()),
            new(ClaimTypes.Email, userSession.Email),
            new(ClaimTypes.Role, userSession.Role) // Injects the role configuration into security identity context
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true, // Remembers session across browser closures
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Hard session limit protection
        };

        // Issues the highly encrypted state cookie to the user's client storage engine
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity), 
            authProperties
        );

        return Ok(new { message = "Login successful!", role = userSession.Role });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Instructs user browser engines to wipe out authorization tracking keys instantly
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Logged out successfully!" });
    }
}