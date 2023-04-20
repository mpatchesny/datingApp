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
        builder.Property(x => x.SwippedById)
            .IsRequired();
        builder.Property(x => x.SwippedWhoId)
            .IsRequired();
        builder.HasIndex(x => new {x.SwippedById, x.SwippedWhoId})
            .IsUnique();
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.SwippedById);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.SwippedWhoId);
    }
}