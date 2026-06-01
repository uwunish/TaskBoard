using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Common.Interfaces
{
	public interface IBoardRepository
	{
		Task<Board?> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<Board?> GetByIdWithColumnsAsync(Guid id, CancellationToken ct = default);
		Task<List<Board>> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct = default);
		Task AddAsync(Board board, CancellationToken ct = default);
		Task UpdateAsync(Board board, CancellationToken ct = default);
		Task DeleteAsync(Guid id, CancellationToken ct = default);
	}
}
