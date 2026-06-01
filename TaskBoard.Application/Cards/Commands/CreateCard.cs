using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Common.Exceptions;
using TaskBoard.Application.Common.Interfaces;

namespace TaskBoard.Application.Cards.Commands
{
    public record CreateCardCommand(
        string Title,
        string? Description,
        Guid ColumnId,
        Guid BoardId,
        Guid CreatedByUserId
        ): IRequest<Guid>;
    public class CreateCardHandler : IRequestHandler<CreateCardCommand, Guid>
    {
        private readonly IBoardRepository _boards;
        private readonly IBoardHubService _hub;

        public CreateCardHandler(IBoardRepository boards, IBoardHubService hub)
        {
            _boards = boards;
            _hub = hub;
        }

        public async Task<Guid> Handle(CreateCardCommand request, CancellationToken ct)
        {
            var board = await _boards.GetByIdWithColumnsAsync(request.BoardId, ct); 
            if(board is null)
            {
                throw new NotFoundException("Board", request.BoardId);
            }

            var column = board.Columns.FirstOrDefault(x => x.Id == request.ColumnId);
            if(column is null)
            {
                throw new NotFoundException("Column", request.ColumnId);
            }

            var card = column.AddCard(request.Title, request.CreatedByUserId, request.Description);
            await _boards.UpdateAsync(board, ct);
            await _hub.CardCreatedAsync(request.BoardId, request.ColumnId, card.Id, card.Title);
            return card.Id;
        }
    }
}
