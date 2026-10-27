[Fact]
public async Task RegisterAsync_ValidRequest_CreatesUserWithUserRoleAndHashedPassword()
{
    User? saved = null;
    _userRepository
        .Setup(repository => repository.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(false);
    _userRepository
        .Setup(repository => repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
        .Callback<User, CancellationToken>((user, _) => saved = user)
        .Returns(Task.CompletedTask);
    var request = new RegisterRequestDto
    {
        FirstName = " Thandi ",
        Surname = "Nkosi",
        Email = " Thandi.Nkosi@Kiosk.Test ",
        MobileNumber = "0821234567",
        Password = CorrectPassword,
    };

    var response = await _sut.RegisterAsync(request);

    var created = Assert.IsType<User>(saved);
    Assert.Equal(UserRole.User, created.Role);
    Assert.Equal("thandi.nkosi@kiosk.test", created.Email);
    Assert.Equal("Thandi", created.FirstName);
    Assert.NotEqual(CorrectPassword, created.PasswordHash);
    Assert.Equal(
        PasswordVerificationResult.Success,
        _passwordHasher.VerifyHashedPassword(created, created.PasswordHash, CorrectPassword));
    Assert.Equal(UserRole.User, response.Role);
}

[Fact]
public async Task RegisterAsync_EmailAlreadyRegistered_ThrowsAndSavesNothing()
{
    _userRepository
        .Setup(repository => repository.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);
    var request = new RegisterRequestDto
    {
        FirstName = "Thandi",
        Surname = "Nkosi",
        Email = "thandi.nkosi@kiosk.test",
        MobileNumber = "0821234567",
        Password = CorrectPassword,
    };

    await Assert.ThrowsAsync<EmailAlreadyExistsException>(() => _sut.RegisterAsync(request));

    _userRepository.Verify(
        repository => repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
        Times.Never);
}

[Fact]
public void RegisterRequestDto_HasNoRoleProperty_SoNobodyCanRegisterAsAdministrator()
{
    var propertyNames = typeof(RegisterRequestDto).GetProperties().Select(property => property.Name);

    Assert.DoesNotContain("Role", propertyNames);
}
