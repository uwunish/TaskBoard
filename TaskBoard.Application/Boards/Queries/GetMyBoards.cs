using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Common.Interfaces;

namespace TaskBoard.Application.Boards.Queries
{
    public record BoardSummaryDto(Guid Id, string Name, string? Description, int ColumnCount);
    public record GetMyBoardsQuery(Guid UserId) : IRequest<List<BoardSummaryDto>>;
    public class GetMyBoardsHandler : IRequestHandler<GetMyBoardsQuery, List<BoardSummaryDto>>
    {
        private readonly IBoardRepository _boards;

        public GetMyBoardsHandler(IBoardRepository boards)
        {
            _boards = boards;
        }

        public async Task<List<BoardSummaryDto>> Handle(GetMyBoardsQuery request, CancellationToken ct)
        {
            var boards = await _boards.GetByOwnerIdAsync(request.UserId, ct);
            return boards
                .Select(x => new BoardSummaryDto(x.Id, x.Name, x.Description, x.Columns.Count))
                .ToList();
        }
    }
}
