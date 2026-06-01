using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Common.Interfaces;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Persistence;

namespace TaskBoard.Infrastructure.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly AppDbContext _db;

        public CardRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Card>> GetByColumnIdAsync(Guid columnId, CancellationToken ct = default)
        {
            return await _db.Cards
                .Where(c => c.ColumnId == columnId)
                .OrderBy(c => c.Position)
                .ToListAsync(ct);
        }

        public async Task<Card?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _db.Cards.FindAsync([id], ct);
        }

        public async Task UpdateAsync(Card card, CancellationToken ct = default)
        {
            _db.Cards.Update(card);
            await _db.SaveChangesAsync(ct);
        }
    }
}
