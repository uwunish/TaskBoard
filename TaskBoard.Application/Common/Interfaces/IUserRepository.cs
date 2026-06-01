using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Common.Interfaces
{
	public interface IUserRepository
	{
		Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
		Task AddAsync(User user, CancellationToken ct = default);
		Task UpdateAsync(User user, CancellationToken ct = default);
	}
}
