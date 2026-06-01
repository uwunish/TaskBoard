using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using MediatR;
using TaskBoard.Application.Common.Exceptions;
using TaskBoard.Application.Common.Interfaces;

namespace TaskBoard.Application.Boards.Queries
{

    // Response DTO - what gets returned to the frontend
    public record CardDto(Guid Id, string Title, string? Description, int Position, Guid? AssigneeId);
    public record ColumnDto(Guid Id, string Title, int Position, List<CardDto> Cards);
    public record BoardDto(Guid Id, string Name, string? Description, List<ColumnDto> Columns);

    // The Query - what the user is asking for
    public record GetBoardQuery(Guid BoardId, Guid RequestingUserId) : IRequest<BoardDto>;

    public class GetBoardHandler : IRequestHandler<GetBoardQuery, BoardDto>
    {
        private readonly IBoardRepository _boards;

        public GetBoardHandler(IBoardRepository boards)
        {
            _boards = boards;
        }

        public async Task<BoardDto> Handle(GetBoardQuery request, CancellationToken ct)
        {
            var board = await _boards.GetByIdWithColumnsAsync(request.BoardId, ct);
            if (board == null)
            {
                throw new NotFoundException("Board", request.BoardId);
            }

            // only the owner can view for now
            if (board.OwnerId != request.RequestingUserId)
            {
                throw new ForbiddenException();
            }

            return new BoardDto(
                    board.Id,
                    board.Name,
                    board.Description,
                    board.Columns
                    .OrderBy(c => c.Position)
                    .Select(c => new ColumnDto(
                            c.Id,
                            c.Title,
                            c.Position,
                            c.Cards
                            .OrderBy(card => card.Position)
                            .Select(card => new CardDto(
                                    card.Id,
                                    card.Title,
                                    card.Description,
                                    card.Position,
                                    card.AssigneeId
                                    )
                            )
                            .ToList()
                            ))
                    .ToList()
             );
        }
    }
}
