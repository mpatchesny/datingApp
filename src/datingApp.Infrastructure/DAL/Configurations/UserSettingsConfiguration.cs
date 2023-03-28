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
        builder.HasKey(e => e.UserId);
        builder.Property(x => x.UserId)
            .IsRequired();
        builder.Property(x => x.DiscoverRange)
            .IsRequired();
        builder.Property(x => x.DiscoverAgeFrom)
            .IsRequired();
        builder.Property(x => x.DiscoverAgeTo)
            .IsRequired();
        builder.Property(x => x.DiscoverSex)
            .IsRequired();
        builder.Property(x => x.Lat)
            .IsRequired();
        builder.Property(x => x.Lon)
            .IsRequired();
    }
}