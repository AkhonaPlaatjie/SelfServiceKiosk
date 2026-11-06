using System.ComponentModel.DataAnnotations;
using SelfServiceKiosk.Api.Validation;

namespace SelfServiceKiosk.Api.DTOs.Auth;

/// <summary>
/// What the client sends to register: the shared personal details plus a password.
/// There is deliberately no Role property: every public registration becomes a User,
/// so nobody can register as an Administrator.
/// </summary>
public class RegisterRequestDto : PersonalDetailsRequestDto
{
    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
    [StrongPassword]
    public string Password { get; set; } = string.Empty;
}
