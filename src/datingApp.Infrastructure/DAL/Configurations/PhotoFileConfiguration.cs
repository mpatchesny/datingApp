using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class PhotoFileConfiguration : IEntityTypeConfiguration<PhotoFile>
{
    public void Configure(EntityTypeBuilder<PhotoFile> builder)
    {
        builder.HasKey(x => x.PhotoId);
        builder.Property(x => x.PhotoId)
            .IsRequired();
        builder.Property(x => x.Content)
            .IsRequired();
        // builder.HasOne<Photo>()
        //     .WithOne()
        //     .HasForeignKey<Photo>(x => x.Id);
    }
}