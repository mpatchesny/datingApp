using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class RevokedTokensConfiguration : IEntityTypeConfiguration<JwtDto>
{
    public void Configure(EntityTypeBuilder<JwtDto> builder)
    {
        builder.Property(x => x.AccessToken)
            .IsRequired();
        builder.Property(x => x.ExpirationTime)
            .IsRequired();
        builder.HasKey(x => x.AccessToken);
        builder.HasIndex(x => new {x.AccessToken});
    }
}