using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Cards.Commands;
using TaskBoard.Application.Common.Exceptions;
using TaskBoard.Domain.Entities;
using TaskBoard.UnitTests.Common;

namespace TaskBoard.UnitTests.Cards
{
    public class MoveCardHandlerTests : TestBase
    {
        private readonly MoveCardHandler _handler;

        public MoveCardHandlerTests()
        {
            _handler = new MoveCardHandler(
                MockCardRepo.Object,
                MockBoardRepo.Object,
                MockHub.Object
                );
        }

        // Helper - builds a real board with columns and a card
        private (Board board, Column sourceColumn, Column targetColumn, Card card) BuildBoardWithCard()
        {
            var ownerId = Guid.NewGuid();
            var board = Board.Create("Test Board", ownerId);
            var sourceColumn = board.AddColumn("To Do");
            var targetColumn = board.AddColumn("In Progress");
            var card = sourceColumn.AddCard("Test Card", ownerId);
            return (board, sourceColumn, targetColumn, card);
        }

        [Fact]
        public async Task Handle_ValidMove_UpdatesCardRepository()
        {
            // Arrange
            var (board, _, targetColumn, card) = BuildBoardWithCard();

            MockCardRepo.Setup(r => r.GetByIdAsync(card.Id, default)).ReturnsAsync(card);
            MockBoardRepo.Setup(r => r.GetByIdWithColumnsAsync(board.Id, default)).ReturnsAsync(board);

            var command = new MoveCardCommand(card.Id, targetColumn.Id, 0, board.Id, Guid.NewGuid());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            MockCardRepo.Verify(
                r => r.UpdateAsync(
                    It.Is<Card>(c => c.ColumnId == targetColumn.Id),
                    It.IsAny<CancellationToken>()
                    ),
                Times.Once
                );
        }

        [Fact]
        public async Task Handle_ValidMove_BroadcastsToSignalR()
        {
            // Arrange
            var (board, _, targetColumn, card) = BuildBoardWithCard();

            MockCardRepo.Setup(r => r.GetByIdAsync(card.Id, default)).ReturnsAsync(card);
            MockBoardRepo.Setup(r => r.GetByIdWithColumnsAsync(board.Id, default)).ReturnsAsync(board);

            var command = new MoveCardCommand(card.Id, targetColumn.Id, 0, board.Id, Guid.NewGuid());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            MockHub.Verify(
                h => h.CardMovedAsync(board.Id, card.Id, targetColumn.Id, 0),
                Times.Once
                );
        }

        [Fact]
        public async Task Handle_CardNotFound_ThrowsNotFoundException()
        {
            // Arrange - repo returns null (card doesn't exist)
            MockCardRepo.Setup(
                r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Card?)null);

            var command = new MoveCardCommand(
                Guid.NewGuid(), Guid.NewGuid(), 0, Guid.NewGuid(), Guid.NewGuid()
                );

            // Act
            var result = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<NotFoundException>();

        }

        [Fact]
        public async Task Handle_TargetColumnNotOnBoard_ThrowsNotFoundException()
        {
            // Arrange - board exists but the target column belongs to a different board
            var (board, _, _, card) = BuildBoardWithCard();
            var wrongColumnId = Guid.NewGuid();

            MockCardRepo.Setup(r => r.GetByIdAsync(card.Id, default)).ReturnsAsync(card);
            MockBoardRepo.Setup(r => r.GetByIdWithColumnsAsync(board.Id, default)).ReturnsAsync(board);

            var command = new MoveCardCommand(card.Id, wrongColumnId, 0, board.Id, Guid.NewGuid());

            // Act
            var result = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<NotFoundException>();

        }

    }
}
