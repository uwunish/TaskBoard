using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Domain.Events
{
	public record CardCreatedEvent
		(
		 Guid CardId,
		 Guid ColumnId,
		 Guid BoardId,
		 Guid CreatedByUserId
		 );
}
