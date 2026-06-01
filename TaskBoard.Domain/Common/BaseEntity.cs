using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Domain.Common
{
	public abstract class BaseEntity
	{
		public Guid Id { get; protected set; } = Guid.NewGuid();
		private readonly List<object> _domainEvents = [];
		public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

		protected void AddDomainEvent(object domainEvent) => _domainEvents.Add(domainEvent);

		public void ClearDomainEvents() => _domainEvents.Clear();
	}
}
