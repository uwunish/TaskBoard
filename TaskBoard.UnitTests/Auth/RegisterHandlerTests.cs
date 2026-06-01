using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Auth.Commands;
using TaskBoard.Domain.Entities;
using TaskBoard.UnitTests.Common;

namespace TaskBoard.UnitTests.Auth
{
    public class RegisterHandlerTests:TestBase
    {
        private readonly RegisterHandler _handler;

        public RegisterHandlerTests()
        {
            // Set up default mock behaviour
            MockHasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed-password");
            MockTokens.Setup(t => t.GenerateAccessToken(It.IsAny<User>())).Returns("access-token");
            MockTokens.Setup(t => t.GenerateRefreshToken()).Returns("refresh-token");

            _handler = new RegisterHandler(MockUserRepo.Object, MockTokens.Object, MockHasher.Object);
        }

        [Fact]
        public async Task Handle_NewEmail_ReturnsTokens()
        {
            // Arrange - no existing user with this email
            MockUserRepo.Setup(u => u.GetByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync((User?)null);

            var command = new RegisterCommand("test@example.com", "Password1", "Test User");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.AccessToken.Should().Be("access-token");
            result.RefreshToken.Should().Be("refresh-token");
            result.DisplayName.Should().Be("Test User");
        }

        [Fact]
        public async Task Handle_NewEmail_SavesUserToRepository()
        {
            // Arrange
            MockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), default))
                .ReturnsAsync((User?)null);

            var command = new RegisterCommand("test@example.com", "Password1", "Test User");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            MockUserRepo.Verify(
                r => r.AddAsync(
                    It.Is<User>(u => u.Email == "test@example.com"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_NewEmail_HashesPassword()
        {
            // Arrange
            MockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), default))
                .ReturnsAsync((User?)null);

            var command = new RegisterCommand("test@example.com", "Password1", "Test User");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert — raw password was never stored, hasher was called
            MockHasher.Verify(h => h.Hash("Password1"), Times.Once);
        }

        [Fact]
        public async Task Handle_DuplicateEmail_ThrowsInvalidOperationException()
        {
            // Arrange — email already exists
            var existingUser = User.Create("test@example.com", "Existing", "hash");
            MockUserRepo.Setup(r => r.GetByEmailAsync("test@example.com", default))
                .ReturnsAsync(existingUser);

            var command = new RegisterCommand("test@example.com", "Password1", "Test User");

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*already registered*");
        }
    }
}
