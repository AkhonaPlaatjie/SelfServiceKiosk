using SelfServiceKiosk.Api.Enums;
using SelfServiceKiosk.Api.Mappers;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Tests.Mappers;

public class UserMapperTests
{
    [Fact]
    public void ToSummaryDto_Account_CopiesTheSafeFields()
    {
        var account = TestUsers.Create(UserRole.SuperUser, id: 7);

        var dto = account.ToSummaryDto();

        Assert.Equal(7, dto.Id);
        Assert.Equal(account.FirstName, dto.FirstName);
        Assert.Equal(account.Surname, dto.Surname);
        Assert.Equal(account.Email, dto.Email);
        Assert.Equal(UserRole.SuperUser, dto.Role);
    }

    [Fact]
    public void ToLoginResponseDto_AccountAndToken_CombinesBoth()
    {
        var account = TestUsers.Create();
        var expiry = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var dto = account.ToLoginResponseDto(new TokenResult("abc", expiry));

        Assert.Equal("abc", dto.AccessToken);
        Assert.Equal(expiry, dto.ExpiresAtUtc);
        Assert.Equal(account.Id, dto.User.Id);
    }
}
