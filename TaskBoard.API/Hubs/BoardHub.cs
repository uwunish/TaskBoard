using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TaskBoard.Infrastructure.Auth;

namespace TaskBoard.API.Hubs
{
    [Authorize]
    public class BoardHub : Hub
    {
        // called by angular when user opens a board
        public async Task JoinBoard(string boardId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, boardId);

            // tell everyone on this board this user joined
            await Clients.OthersInGroup(boardId).SendAsync("UserJoined", new
            {
                UserId = GetUserId(),
                DisplayName = GetDisplayName()
            });
        }

        // called by angular when user leaves a board
        public async Task LeaveBoard(string boardId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, boardId);

            await Clients.OthersInGroup(boardId).SendAsync("UserLeft", new
            {
                UserId = GetUserId()
            });
        }


        // fires when a user disconnects (closes tab, loses connection)
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // angular will call LeaveBoard before disconnecting in normal flow
            // This handles unexpected disconnections
            await base.OnDisconnectedAsync(exception);
        }

        private string GetUserId()
        {
            return Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Context.User?.FindFirstValue("sub")
                ?? "unknown";
        }

        private string GetDisplayName()
        {
            return Context.User?.FindFirstValue("displayName") ?? "Unknown";
        }
    }
}
