namespace SelfServiceKiosk.Api.Validation;

/// <summary>South African mobile number rules, shared by registration and profile.</summary>
public static class MobileNumberRules
{
    // 0 or +27, then 6, 7 or 8, then 8 more digits (for example 0821234567 or +27821234567).
    // [0-9] and \z are used on purpose: \d also matches non-Latin digits, and $ allows a trailing newline.
    public const string Pattern = @"^(\+27|0)[6-8][0-9]{8}\z";

    public const string Message =
        "Mobile number must be a valid South African number, for example 0821234567 or +27821234567.";
}
