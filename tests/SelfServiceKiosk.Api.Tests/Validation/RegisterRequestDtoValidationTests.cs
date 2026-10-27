using System.ComponentModel.DataAnnotations;
using SelfServiceKiosk.Api.DTOs.Auth;

namespace SelfServiceKiosk.Api.Tests.Validation;

/// <summary>
/// Unit tests for each registration validation rule. The DataAnnotations rules are run directly,
/// so no web server or database is needed. All data is fake.
/// </summary>
public class RegisterRequestDtoValidationTests
{
    [Fact]
    public void Validate_ValidRequest_HasNoErrors()
    {
        Assert.Empty(Validate(ValidRequest()));
    }

    [Theory]
    [InlineData(nameof(RegisterRequestDto.FirstName))]
    [InlineData(nameof(RegisterRequestDto.Surname))]
    [InlineData(nameof(RegisterRequestDto.Email))]
    [InlineData(nameof(RegisterRequestDto.MobileNumber))]
    [InlineData(nameof(RegisterRequestDto.Password))]
    public void Validate_RequiredFieldMissing_ReportsThatField(string propertyName)
    {
        var request = ValidRequest();
        typeof(RegisterRequestDto).GetProperty(propertyName)!.SetValue(request, string.Empty);

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(propertyName));
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("user@@kiosk.test")]
    [InlineData("@kiosk.test")]
    public void Validate_InvalidEmail_ReportsEmail(string email)
    {
        var request = ValidRequest();
        request.Email = email;

        Assert.Contains(Validate(request), error => error.MemberNames.Contains(nameof(RegisterRequestDto.Email)));
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("0521234567")]
    [InlineData("082123456")]
    [InlineData("08212345678")]
    [InlineData("+27 82 123 4567")]
    [InlineData("abcdefghij")]
    public void Validate_InvalidMobileNumber_ReportsMobileNumber(string mobileNumber)
    {
        var request = ValidRequest();
        request.MobileNumber = mobileNumber;

        Assert.Contains(Validate(request), error => error.MemberNames.Contains(nameof(RegisterRequestDto.MobileNumber)));
    }

    [Theory]
    [InlineData("0821234567")]
    [InlineData("+27821234567")]
    [InlineData("0731234567")]
    [InlineData("0611234567")]
    public void Validate_ValidSouthAfricanMobileNumber_HasNoErrors(string mobileNumber)
    {
        var request = ValidRequest();
        request.MobileNumber = mobileNumber;

        Assert.Empty(Validate(request));
    }

    [Theory]
    [InlineData("password")]
    [InlineData("PASSWORD1!")]
    [InlineData("password1!")]
    [InlineData("Password!!")]
    [InlineData("Password12")]
    [InlineData("Pa1!")]
    public void Validate_WeakPassword_ReportsPassword(string password)
    {
        var request = ValidRequest();
        request.Password = password;

        Assert.Contains(Validate(request), error => error.MemberNames.Contains(nameof(RegisterRequestDto.Password)));
    }

    [Fact]
    public void Validate_FirstNameLongerThanColumn_ReportsFirstName()
    {
        var request = ValidRequest();
        request.FirstName = new string('a', 101);

        Assert.Contains(Validate(request), error => error.MemberNames.Contains(nameof(RegisterRequestDto.FirstName)));
    }

    [Fact]
    public void Validate_SurnameLongerThanColumn_ReportsSurname()
    {
        var request = ValidRequest();
        request.Surname = new string('a', 101);

        Assert.Contains(Validate(request), error => error.MemberNames.Contains(nameof(RegisterRequestDto.Surname)));
    }

    [Fact]
    public void Validate_EmailLongerThanColumn_ReportsEmail()
    {
        var request = ValidRequest();
        request.Email = new string('a', 250) + "@kiosk.test";

        Assert.Contains(Validate(request), error => error.MemberNames.Contains(nameof(RegisterRequestDto.Email)));
    }

    [Fact]
    public void Validate_PasswordLongerThanLimit_ReportsPassword()
    {
        var request = ValidRequest();
        request.Password = "Aa1!" + new string('a', 97);

        Assert.Contains(Validate(request), error => error.MemberNames.Contains(nameof(RegisterRequestDto.Password)));
    }

    [Fact]
    public void Validate_WeakPassword_MessageNeverEchoesThePassword()
    {
        var request = ValidRequest();
        request.Password = "weakmarker";

        var messages = Validate(request).Select(error => error.ErrorMessage ?? string.Empty);

        Assert.All(messages, message => Assert.DoesNotContain("weakmarker", message));
    }

    private static RegisterRequestDto ValidRequest()
    {
        return new RegisterRequestDto
        {
            FirstName = "Thandi",
            Surname = "Nkosi",
            Email = "thandi.nkosi@kiosk.test",
            MobileNumber = "0821234567",
            Password = "Str0ng!Passw0rd",
        };
    }

    private static List<ValidationResult> Validate(RegisterRequestDto request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
        return results;
    }
}
