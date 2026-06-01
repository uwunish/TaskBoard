using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Common.Interfaces;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Boards.Commands
{
    // The Command - what the user is asking to do
    public record CreateBoardCommand(string Name, string? Description, Guid OwnerId) : IRequest<Guid>;

    // The handler - how we do it
    public class CreateBoardHandler : IRequestHandler<CreateBoardCommand, Guid>
    {
        private readonly IBoardRepository _boards;

        public CreateBoardHandler(IBoardRepository boards)
        {
            _boards = boards;
        }

        public async Task<Guid> Handle(CreateBoardCommand request, CancellationToken ct)
        {
            var board = Board.Create(request.Name, request.OwnerId, request.Description);

            // Add a default Columns so the board is not empty when created
            board.AddColumn("To Do");
            board.AddColumn("In Progress");
            board.AddColumn("Done");

            await _boards.AddAsync(board, ct);
            return board.Id;
        }
    }
}
