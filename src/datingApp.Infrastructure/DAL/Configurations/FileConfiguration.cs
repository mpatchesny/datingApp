using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class FileConfiguration : IEntityTypeConfiguration<FileDto>
{
    public void Configure(EntityTypeBuilder<FileDto> builder)
    {
        builder.Property(x => x.Id)
            .IsRequired();
        builder.Property(x => x.Extension)
            .IsRequired();
        builder.Property(x => x.Binary)
            .IsRequired();
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new {x.Id});
        builder.HasOne<Photo>()
               .WithOne()
               .HasForeignKey<FileDto>(x => x.Id);
    }
}