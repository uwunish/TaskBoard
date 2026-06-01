using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Common.Interfaces;
using TaskBoard.Infrastructure.Auth;

namespace TaskBoard.Infrastructure.Persistence
{
    public class BoardHubService : IBoardHubService
    {
        // IHubContext lets you push messages to SignalR clients
        // from outside the hub - e.g from a MediatR handler
        private readonly IHubContext<BoardHubMarker> _hubContext;

        public BoardHubService(IHubContext<BoardHubMarker> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BoardUpdatedAsync(Guid boardId)
        {
            await _hubContext.Clients.Group(boardId.ToString())
                .SendAsync("BoardUpdated", new { boardId });
        }

        public async Task CardCreatedAsync(Guid boardId, Guid columnId, Guid cardId, string title)
        {
            await _hubContext.Clients.Group(boardId.ToString())
                .SendAsync("CardCreated", new { cardId, columnId, title });
        }

        public async Task CardMovedAsync(Guid boardId, Guid cardId, Guid targetColumnId, int newPosition)
        {
            Console.WriteLine($"Broadcasting CardMoved to board {boardId}");
            await _hubContext.Clients.Group(boardId.ToString())
                .SendAsync("CardMoved", new { cardId, targetColumnId, newPosition });
        }
    }
}
