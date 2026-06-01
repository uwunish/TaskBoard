using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Common.Interfaces
{
	public interface ICardRepository
	{
		Task<Card?> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task UpdateAsync(Card card, CancellationToken ct = default);
		Task<List<Card>> GetByColumnIdAsync(Guid columnId, CancellationToken ct = default);
	}
}
