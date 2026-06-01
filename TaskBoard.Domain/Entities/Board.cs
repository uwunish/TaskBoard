using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Domain.Entities
{
	public class Board : Common.BaseEntity
	{
		public string Name { get; private set; } = string.Empty;
		public string? Description { get; private set; }
		public Guid OwnerId { get; private set; }
		public DateTime CreatedAt { get; private set; }

		private readonly List<Column> _columns = [];
		public IReadOnlyList<Column> Columns => _columns.AsReadOnly();

		private Board() { }

		public static Board Create(string name, Guid ownerId, string? description = null)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(name);
			return new Board
			{
				Name = name.Trim(),
				Description = description?.Trim(),
				OwnerId = ownerId,
				CreatedAt = DateTime.UtcNow
			};
		}

		public Column AddColumn(string title)
		{
			var position = _columns.Count;
			var column = Column.Create(title, Id, position);
			_columns.Add(column);
			return column;
		}

		public void Rename(string newName)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(newName);
			Name = newName.Trim();
		}
	}
}
