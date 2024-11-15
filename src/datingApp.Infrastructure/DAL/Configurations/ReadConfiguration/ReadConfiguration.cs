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
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);
        builder.Ignore(user => user.Distance);
        builder
            .HasOne(user => user.Settings)
            .WithOne(us => us.User)
            .HasForeignKey<UserSettingsReadModel>(us => us.UserId);
        builder
            .HasMany(user => user.Matches1)
            .WithOne(match => match.User1);
        builder
            .HasMany(user => user.Matches2)
            .WithOne(match => match.User2);
    }

    public void Configure(EntityTypeBuilder<UserSettingsReadModel> builder)
    {
        builder.ToTable("UserSettings");
        builder.HasKey(us => us.UserId);
    }

    public void Configure(EntityTypeBuilder<PhotoReadModel> builder)
    {
        builder.ToTable("Photos");
        builder.HasKey(photo => photo.Id);
    }

    public void Configure(EntityTypeBuilder<MatchReadModel> builder)
    {
        builder.ToTable("Matches");
        builder.HasKey(match => match.Id);
        builder.Ignore(match => match.LastChangeTime);
    }

    public void Configure(EntityTypeBuilder<MessageReadModel> builder)
    {
        builder.ToTable("Messages");
        builder.HasKey(message => message.Id);
    }
}