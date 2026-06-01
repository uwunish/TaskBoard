using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Domain.Entities
{
	public class Card : Common.BaseEntity
	{
		public string Title { get; private set; } = string.Empty;
		public string? Description { get; private set; }
		public Guid ColumnId { get; private set; }
		public int Position { get; private set; }
		public Guid CreatedByUserId { get; private set; }
		public Guid? AssigneeId { get; private set; }
		public DateTime CreatedAt { get; private set; }

		private Card() { }

		internal static Card Create(
				string title,
				Guid columnId,
				int position,
				Guid createdByUserId,
				string? description = null
				)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(title);
			return new Card
			{
				Title = title.Trim(),
				Description = description?.Trim(),
				ColumnId = columnId,
				Position = position,
				CreatedByUserId = createdByUserId,
				CreatedAt = DateTime.UtcNow
			};
		}

		public void MoveTo(Guid targetColumnId, int newPosition)
		{
			ColumnId = targetColumnId;
			Position = newPosition;
		}

		public void Assign(Guid userId)
		{
			AssigneeId = userId;
		}

		public void Unassign()
		{
			AssigneeId = null;
		}

		public void Update(string title, string? description)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(title);
			Title = title.Trim();
			Description = description?.Trim();
		}
	}
}
