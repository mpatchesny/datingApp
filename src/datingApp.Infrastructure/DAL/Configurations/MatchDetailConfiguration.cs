using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class MatchDetailConfiguration : IEntityTypeConfiguration<MatchDetail>
{
    public void Configure(EntityTypeBuilder<MatchDetail> builder)
    {
        builder.Property(x => x.MatchId)
            .HasConversion(x => x.Value, x => new MatchId(x))
            .IsRequired();
        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => new UserId(x))
            .IsRequired();
        builder.Property(x => x.IsDisplayed)
            .IsRequired();
        builder.HasKey(x => new {x.MatchId, x.UserId});
    }
}
