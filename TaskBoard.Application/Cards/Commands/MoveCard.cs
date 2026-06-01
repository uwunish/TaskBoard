using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Common.Exceptions;
using TaskBoard.Application.Common.Interfaces;

namespace TaskBoard.Application.Cards.Commands
{
    public record MoveCardCommand(
        Guid CardId,
        Guid TargetColumnId,
        int NewPosition,
        Guid BoardId,
        Guid MovedByUserId
        ) : IRequest;
    public class MoveCardHandler : IRequestHandler<MoveCardCommand>
    {
        private readonly ICardRepository _cards;
        private readonly IBoardRepository _boards;
        private readonly IBoardHubService _hub;

        public MoveCardHandler(
            ICardRepository cards,
            IBoardRepository boards,
            IBoardHubService hub
            )
        {
            _cards = cards;
            _boards = boards;
            _hub = hub;
        }

        public async Task Handle(MoveCardCommand request, CancellationToken ct)
        {
            var card = await _cards.GetByIdAsync(request.CardId, ct);
            if (card == null)
            {
                throw new NotFoundException(nameof(card), request.CardId);
            }

            var board = await _boards.GetByIdWithColumnsAsync(request.BoardId, ct);
            if (board == null)
            {
                throw new NotFoundException(nameof(board), request.BoardId);
            }

            // target column must belong to this board
            var targetColumn = board.Columns.FirstOrDefault(c => c.Id == request.TargetColumnId);
            if (targetColumn == null)
            {
                throw new NotFoundException("Column", request.TargetColumnId);
            }

            var sourceColumnId = card.ColumnId;

            // Delegate the move to the domain entity
            card.MoveTo(request.TargetColumnId, request.NewPosition);
            await _cards.UpdateAsync(card, ct);

            // Notify all connected users on this board via SignalR
            await _hub.CardMovedAsync(
                request.BoardId,
                request.CardId,
                request.TargetColumnId,
                request.NewPosition
                );

        }

    }
}
