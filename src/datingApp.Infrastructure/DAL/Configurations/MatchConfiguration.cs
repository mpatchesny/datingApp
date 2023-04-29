using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .IsRequired();
        builder.Property(x => x.UserId1)
            .IsRequired();
        builder.Property(x => x.UserId2)
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.Property(x => x.IsDisplayedByUser1)
            .IsRequired();
        builder.Property(x => x.IsDisplayedByUser2)
            .IsRequired();
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId1);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId2);

        builder.HasIndex(x => new {x.UserId1, x.UserId2, x.CreatedAt});
    }
}