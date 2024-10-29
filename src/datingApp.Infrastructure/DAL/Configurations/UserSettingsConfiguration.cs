using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => new UserId(x))
            .IsRequired();
        builder.Property(x => x.PreferredMaxDistance)
            .HasConversion(x => x.Value, x => new PreferredMaxDistance(x))
            .IsRequired();
        builder.Property(x => x.PreferredSex)
            .IsRequired();
        builder.OwnsOne(e => e.PreferredAge);
        builder.OwnsOne(e => e.Location);
        // builder.HasIndex(x => x.Location);
    }
}