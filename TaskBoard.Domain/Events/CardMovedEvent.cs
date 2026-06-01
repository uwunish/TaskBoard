using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Domain.Events
{
	public record CardMovedEvent
	(
		Guid CardId,
		Guid SourceColumnId,
		Guid TargetColumnId,
		int NewPosition,
		Guid BoardId,
		Guid MovedByUserId
	);
}
