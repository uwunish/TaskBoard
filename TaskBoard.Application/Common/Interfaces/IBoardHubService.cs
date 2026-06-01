using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Application.Common.Interfaces
{
	public interface IBoardHubService
	{
		Task CardMovedAsync(Guid boardId, Guid cardId, Guid targetColumnId, int newPosition);
		Task CardCreatedAsync(Guid boardId, Guid columnId, Guid cardId, string title);
		Task BoardUpdatedAsync(Guid boardId);
	}
}
