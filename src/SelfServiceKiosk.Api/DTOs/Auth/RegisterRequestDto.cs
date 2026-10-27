using System.ComponentModel.DataAnnotations;
using SelfServiceKiosk.Api.Validation;

namespace SelfServiceKiosk.Api.DTOs.Auth;

/// <summary>
/// What the client sends to register. There is deliberately no Role property:
/// every public registration becomes a User, so nobody can register as an Administrator.
/// </summary>
public class RegisterRequestDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, ErrorMessage = "First name must be at most 100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Surname is required.")]
    [StringLength(100, ErrorMessage = "Surname must be at most 100 characters.")]
    public string Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    [StringLength(256, ErrorMessage = "Email must be at most 256 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mobile number is required.")]
    [RegularExpression(MobileNumberRules.Pattern, ErrorMessage = MobileNumberRules.Message)]
    [StringLength(20, ErrorMessage = "Mobile number must be at most 20 characters.")]
    public string MobileNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
    [StrongPassword]
    public string Password { get; set; } = string.Empty;
}
