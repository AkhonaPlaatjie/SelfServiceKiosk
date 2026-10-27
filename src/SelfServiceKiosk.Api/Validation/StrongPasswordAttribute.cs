using System.ComponentModel.DataAnnotations;

namespace SelfServiceKiosk.Api.Validation;

/// <summary>
/// Password strength: an upper-case letter, a lower-case letter, a digit and a symbol.
/// The message is fixed, so it can never echo the password back (NFR-12).
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class StrongPasswordAttribute : ValidationAttribute
{
    public StrongPasswordAttribute()
        : base("Password must contain an upper-case letter, a lower-case letter, a number and a symbol.")
    {
    }

    public override bool IsValid(object? value)
    {
        // Emptiness is reported by [Required], so a missing value is not reported twice.
        if (value is not string password || password.Length == 0)
        {
            return true;
        }

        return password.Any(char.IsUpper)
            && password.Any(char.IsLower)
            && password.Any(char.IsDigit)
            && password.Any(character => !char.IsLetterOrDigit(character));
    }
}
