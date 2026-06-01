using SecureSessionApi.Data;
using SecureSessionApi.Models.Dtos;
using SecureSessionApi.Models.Entities;
using SecureSessionApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SecureSessionApi.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;

    public AuthService(AppDbContext db) => _db = db;

    public async Task<bool> RegisterAsync(UserRegisterDto dto)
    {
        // Optimization: AsNoTracking eliminates identity map caching overhead
        var userExists = await _db.Users.AsNoTracking().AnyAsync(u => u.Email == dto.Email);
        if (userExists) return false;

        var newUser = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<UserSessionDto?> ValidateUserCredentialsAsync(UserLoginDto dto)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return null; // Structural separation: avoid throwing performance-heavy exceptions
        }

        return new UserSessionDto(user.Id, user.Email, user.Role);
    }
}