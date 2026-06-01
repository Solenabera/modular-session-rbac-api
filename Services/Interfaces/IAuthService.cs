using SecureSessionApi.Models.Dtos;

namespace SecureSessionApi.Services.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(UserRegisterDto dto);
    Task<UserSessionDto?> ValidateUserCredentialsAsync(UserLoginDto dto);
}