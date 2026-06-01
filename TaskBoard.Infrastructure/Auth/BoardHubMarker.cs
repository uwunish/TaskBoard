using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Common.Interfaces;

namespace TaskBoard.Infrastructure.Auth
{
    public class BoardHubMarker : Hub, IBoardHub
    {
    }
}
