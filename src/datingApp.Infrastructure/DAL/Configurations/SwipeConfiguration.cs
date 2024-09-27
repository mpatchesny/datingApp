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
        builder.Property(x => x.SwipedById)
            .IsRequired();
        builder.Property(x => x.SwipedWhoId)
            .IsRequired();
        builder.Property(x => x.Like)
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.HasKey(x => new {x.SwipedById, x.SwipedWhoId});
        builder.HasIndex(x => new {x.SwipedById, x.SwipedWhoId, x.Like})
            .IsUnique();
    }
}