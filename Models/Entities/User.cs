using System.ComponentModel.DataAnnotations;

namespace SecureSessionApi.Models.Entities;

public class User
{
    public int Id { get; set; }

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    // Security Feature: Role designation for RBAC checks (e.g., "User", "Admin")
    [Required, MaxLength(30)]
    public string Role { get; set; } = "User";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}