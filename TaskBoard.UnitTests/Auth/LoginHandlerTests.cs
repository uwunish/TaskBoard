using FluentAssertions;
using Moq;
using TaskBoard.Application.Auth.Commands;
using TaskBoard.Domain.Entities;
using TaskBoard.UnitTests.Common;

namespace TaskBoard.UnitTests.Auth;

public class LoginHandlerTests : TestBase
{
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        MockTokens.Setup(t => t.GenerateAccessToken(It.IsAny<User>())).Returns("access-token");
        MockTokens.Setup(t => t.GenerateRefreshToken()).Returns("refresh-token");

        _handler = new LoginHandler(
            MockUserRepo.Object,
            MockTokens.Object,
            MockHasher.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsTokens()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "hashed");
        MockUserRepo.Setup(r => r.GetByEmailAsync("test@example.com", default))
            .ReturnsAsync(user);
        MockHasher.Setup(h => h.Verify("Password1", "hashed")).Returns(true);

        var command = new LoginCommand("test@example.com", "Password1");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AccessToken.Should().Be("access-token");
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorized()
    {
        // Arrange
        MockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), default))
            .ReturnsAsync((User?)null);

        var command = new LoginCommand("nobody@example.com", "Password1");

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorized()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "hashed");
        MockUserRepo.Setup(r => r.GetByEmailAsync("test@example.com", default))
            .ReturnsAsync(user);
        MockHasher.Setup(h => h.Verify("wrongpassword", "hashed")).Returns(false);

        var command = new LoginCommand("test@example.com", "wrongpassword");

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
