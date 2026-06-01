using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Domain.Entities
{
	public class Column : Common.BaseEntity
	{
		public string Title { get; private set; } = string.Empty;
		public Guid BoardId { get; private set; }
		public int Position { get; private set; }

		private readonly List<Card> _cards = [];
		public IReadOnlyList<Card> Cards => _cards.AsReadOnly();

		private Column() { }

		internal static Column Create(string title, Guid boardId, int position)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(title);
			return new Column
			{
				Title = title.Trim(),
				BoardId = boardId,
				Position = position
			};
		}

		public Card AddCard(string title, Guid createdByUserId, string? description = null)
		{
			var position = _cards.Count;
			var card = Card.Create(title, Id, position, createdByUserId, description);
			_cards.Add(card);

			AddDomainEvent(new Events.CardCreatedEvent(card.Id, Id, BoardId, createdByUserId));
			return card;
		}

		public void UpdatePosition(int newPosition) => Position = newPosition;

		public void Rename(string newTitle)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(newTitle);
			Title = newTitle.Trim();
		}
	}
}
