using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Common.Interfaces;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Persistence;

namespace TaskBoard.Infrastructure.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly AppDbContext _db;

        public BoardRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Board board, CancellationToken ct = default)
        {
            await _db.Boards.AddAsync(board, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var board = await GetByIdAsync(id, ct);
            if (board is not null)
            {
                _db.Boards.Remove(board);
                await _db.SaveChangesAsync(ct);
            }
        }

        public async Task<Board?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _db.Boards.FindAsync([id], ct);
        }

        public async Task<Board?> GetByIdWithColumnsAsync(Guid id, CancellationToken ct = default)
        {
            return await _db.Boards
                .Include(x => x.Columns)
                .ThenInclude(c => c.Cards)
                .FirstOrDefaultAsync(b => b.Id == id, ct);
        }

        public async Task<List<Board>> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct = default)
        {
            return await _db.Boards
                .Include(x => x.Columns)
                .Where(x => x.OwnerId == ownerId)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(Board board, CancellationToken ct = default)
        {

            // _db.Boards.Update(board);
            // await _db.SaveChangesAsync(ct);

            // if EF is already tracking this entity ( i.e. we loaded it in the same request),
            // then just save - don't call Update() which remarks everything as modified
            //if(_db.Entry(board).State == EntityState.Detached)
            //{
            //    _db.Boards.Update(board);
            //}

            //await _db.SaveChangesAsync(ct);

            // Explicitly tell EF about any new cards in the columns
            foreach (var column in board.Columns)
            {
                foreach (var card in column.Cards)
                {
                    if (_db.Entry(card).State == EntityState.Detached)
                        await _db.Cards.AddAsync(card, ct);  //  mark new cards as Added
                }
            }

            await _db.SaveChangesAsync(ct);

        }
    }
}
