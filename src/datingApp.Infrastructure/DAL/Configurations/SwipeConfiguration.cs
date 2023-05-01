using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class SwipeConfiguration : IEntityTypeConfiguration<Swipe>
{
    public void Configure(EntityTypeBuilder<Swipe> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .IsRequired();
        builder.Property(x => x.SwipedById)
            .IsRequired();
        builder.Property(x => x.SwipedWhoId)
            .IsRequired();
        builder.HasIndex(x => new {x.SwipedById, x.SwipedWhoId})
            .IsUnique();
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.SwipedById);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.SwipedWhoId);

        builder.HasIndex(x => new {x.SwipedById, x.SwipedWhoId, x.Like});
    }
}