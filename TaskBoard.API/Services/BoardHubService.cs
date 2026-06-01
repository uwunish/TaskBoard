using Microsoft.AspNetCore.SignalR;
using TaskBoard.API.Hubs;
using TaskBoard.Application.Common.Interfaces;

namespace TaskBoard.API.Services;

public class BoardHubService : IBoardHubService
{
    private readonly IHubContext<BoardHub> _hub;

    public BoardHubService(IHubContext<BoardHub> hub)
        => _hub = hub;

    public async Task CardMovedAsync(Guid boardId, Guid cardId, Guid targetColumnId, int newPosition)
    {
        Console.WriteLine($"Broadcasting CardMoved to board {boardId}");
        await _hub.Clients.Group(boardId.ToString())
            .SendAsync("CardMoved", new { cardId, targetColumnId, newPosition });
    }

    public async Task CardCreatedAsync(Guid boardId, Guid columnId, Guid cardId, string title)
        => await _hub.Clients.Group(boardId.ToString())
            .SendAsync("CardCreated", new { cardId, columnId, title });

    public async Task BoardUpdatedAsync(Guid boardId)
        => await _hub.Clients.Group(boardId.ToString())
            .SendAsync("BoardUpdated", new { boardId });
}