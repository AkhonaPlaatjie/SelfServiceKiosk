using System.ComponentModel.DataAnnotations;
using SelfServiceKiosk.Api.Validation;

namespace SelfServiceKiosk.Api.DTOs.Auth;

/// <summary>
/// The personal details and their validation rules, shared by registration and profile updates
/// so the rules exist in exactly one place. Maximum lengths match the database columns.
/// </summary>
public abstract class PersonalDetailsRequestDto
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
}
