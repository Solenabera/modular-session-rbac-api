using System.ComponentModel.DataAnnotations;

namespace SecureSessionApi.Models.Dtos;

public record UserRegisterDto(
    [Required, EmailAddress] string Email, 
    [Required, MinLength(8)] string Password,
    string Role = "User" // Allows assigning "Admin" for structural simulation
);

public record UserLoginDto(
    [Required, EmailAddress] string Email, 
    [Required] string Password
);

public record UserSessionDto(int Id, string Email, string Role);