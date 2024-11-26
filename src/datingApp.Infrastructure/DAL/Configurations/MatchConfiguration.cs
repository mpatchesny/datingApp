using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new MatchId(x))
            .IsRequired();
        builder.Property(x => x.UserId1)
            .HasConversion(x => x.Value, x => new UserId(x))
            .IsRequired();
        builder.Property(x => x.UserId2)
            .HasConversion(x => x.Value, x => new UserId(x))
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.Navigation(x => x.MatchDetails).AutoInclude();
    }
}