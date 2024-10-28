using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId)
            .IsRequired();
        builder.Property(x => x.PreferredMaxDistance)
            .IsRequired();
        builder.Property(x => x.PreferredAge.From)
            .IsRequired();
        builder.Property(x => x.PreferredAge.To)
            .IsRequired();
        builder.Property(x => x.PreferredSex)
            .IsRequired();
        builder.Property(x => x.Location.Lat)
            .IsRequired();
        builder.Property(x => x.Location.Lon)
            .IsRequired();

        builder.HasIndex(x => new {x.Location.Lat, x.Location.Lon});
    }
}