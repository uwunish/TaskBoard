using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Persistence
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Board> Boards => Set<Board>();
        public DbSet<Column> Columns => Set<Column>();
        public DbSet<Card> Cards => Set<Card>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // picks up all IEntityTypeConfiguration<T> classes automatically
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct =default)
        {
            // automatically clears domain events after saving
            var entitiesWithEvents = ChangeTracker
                .Entries<Domain.Common.BaseEntity>()
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var result = await base.SaveChangesAsync(ct);
            entitiesWithEvents.ForEach(e => e.ClearDomainEvents());
            return result;
        }
    }
}
