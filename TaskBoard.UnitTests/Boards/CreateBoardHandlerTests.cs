using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Boards.Commands;
using TaskBoard.Domain.Entities;
using TaskBoard.UnitTests.Common;

namespace TaskBoard.UnitTests.Boards
{
    public class CreateBoardHandlerTests : TestBase
    {
        private readonly CreateBoardHandler _handler;

        public CreateBoardHandlerTests()
        {
            _handler = new CreateBoardHandler(MockBoardRepo.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsNewBoardId()
        {
            // Arrange
            var command = new CreateBoardCommand("Test Board", "A description", Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Handle_ValidCommand_SavesBoardToRepository()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var command = new CreateBoardCommand("My Board", null, ownerId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            MockBoardRepo.Verify(
                r => r.AddAsync(
                    It.Is<Board>(b => b.Name == "My Board" && b.OwnerId == ownerId),
                    It.IsAny<CancellationToken>()
                    ),
                Times.Once
                );
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesThreeDefaultColumns()
        {
            // Arrange
            var command = new CreateBoardCommand("Board", null, Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            MockBoardRepo.Verify(
                r => r.AddAsync(
                    It.Is<Board>(b => b.Columns.Count == 3),
                    It.IsAny<CancellationToken>()
                    ),
                Times.Once
                );
        }

        [Fact]
        public async Task Handle_EmptyName_ThrowsArgumentException()
        {
            // Arrange
            var command = new CreateBoardCommand("", null, Guid.NewGuid());

            // Act
            var result = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<ArgumentException>();
        }


    }
}
