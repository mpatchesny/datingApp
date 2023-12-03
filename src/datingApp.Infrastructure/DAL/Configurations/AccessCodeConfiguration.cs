using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class AccessCodeConfiguration : IEntityTypeConfiguration<AccessCodeDto>
{
    public void Configure(EntityTypeBuilder<AccessCodeDto> builder)
    {
        builder.Property(x => x.EmailOrPhone)
            .IsRequired();
        builder.Property(x => x.AccessCode)
            .IsRequired();
        builder.Property(x => x.Expiry)
            .IsRequired();
        builder.Property(x => x.ExpirationTime)
            .IsRequired();
        builder.HasKey(x => x.EmailOrPhone);
        builder.HasIndex(x => new {x.EmailOrPhone});
    }
}