using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);
        builder.HasIndex(x => x.Phone).IsUnique();
        builder.Property(x => x.Phone)
            .IsRequired()
            .HasMaxLength(9);
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(15);
        builder.Property(x => x.Bio)
            .HasMaxLength(400);
        builder.Property(x => x.Job)
            .HasMaxLength(30);
        builder.Property(x => x.DateOfBirth)
            .IsRequired();
        builder.Property(x => x.Sex)
            .IsRequired();
    }
}
