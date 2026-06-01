using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Persistence.Configurations
{
    public class ColumnConfiguration : IEntityTypeConfiguration<Column>
    {
        public void Configure(EntityTypeBuilder<Column> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(x => x.Cards)
                .WithOne()
                .HasForeignKey(card => card.ColumnId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(x => x.Cards)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
