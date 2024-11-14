using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations.ReadConfiguration;

internal sealed class ReadConfiguration : IEntityTypeConfiguration<UserReadModel>, IEntityTypeConfiguration<UserSettingsReadModel>,
    IEntityTypeConfiguration<PhotoReadModel>, IEntityTypeConfiguration<MatchReadModel>, IEntityTypeConfiguration<MessageReadModel>
{
    public void Configure(EntityTypeBuilder<UserReadModel> builder)
    {
        builder
            .HasOne<UserSettingsReadModel>()
            .WithOne(us => us.User);

        builder.HasMany(user => user.Photos)
            .WithOne(photo => photo.User)
            .HasForeignKey(photo => photo.User);

        builder.HasMany(user => user.Matches)
            .WithOne(match => match.User)
            .HasForeignKey(match => match.Owner)
            .HasForeignKey(match => match.User);

        builder.ToTable("Users");
    }

    public void Configure(EntityTypeBuilder<UserSettingsReadModel> builder)
    {
        builder.ToTable("UserSettings");
    }

    public void Configure(EntityTypeBuilder<PhotoReadModel> builder)
    {
        builder.ToTable("Photos");
    }

    public void Configure(EntityTypeBuilder<MatchReadModel> builder)
    {
        builder.HasMany(match => match.Messages)
            .WithOne(message => message.Match)
            .HasForeignKey(message => message.Match);

        builder.ToTable("Matches");
    }

    public void Configure(EntityTypeBuilder<MessageReadModel> builder)
    {
        builder.ToTable("Messages");
    }
}