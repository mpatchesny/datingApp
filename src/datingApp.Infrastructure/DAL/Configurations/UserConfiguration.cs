using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new UserId(x))
            .IsRequired();
        builder.Property(x => x.Email)
            .HasConversion(x => x.Value, x => new Email(x))
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(x => x.Phone)
            .HasConversion(x => x.Value, x => new Phone(x))
            .IsRequired()
            .HasMaxLength(9);
        builder.Property(x => x.Name)
            .HasConversion(x => x.Value, x => new Name(x))
            .IsRequired()
            .HasMaxLength(15);
        builder.Property(x => x.Bio)
            .HasConversion(x => x.Value, x => new Bio(x))
            .HasMaxLength(400);
        builder.Property(x => x.Job)
            .HasConversion(x => x.Value, x => new Job(x))
            .HasMaxLength(50);
        builder.Property(x => x.DateOfBirth)
            .HasConversion(x => x.Value, x => new DateOfBirth(x))
            .IsRequired();
        builder.Property(x => x.Sex)
            .IsRequired();
        builder.HasMany(u => u.Matches)
            .WithMany(m => m.Users)
            .UsingEntity<MatchDetail>();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => new {x.Sex, x.DateOfBirth});
    }
}
