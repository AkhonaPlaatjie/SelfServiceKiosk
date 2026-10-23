using System.ComponentModel.DataAnnotations;

namespace SelfServiceKiosk.Api.DTOs.Auth;

/// <summary>
/// What the client sends to register. There is deliberately no Role property:
/// every public registration becomes a User, so nobody can register as an Administrator.
/// </summary>
public class RegisterRequestDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string Surname { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string MobileNumber { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
