using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class RevokedRefreshTokenConfiguration : IEntityTypeConfiguration<TokenDto>
{
    public void Configure(EntityTypeBuilder<TokenDto> builder)
    {
        builder.Property(x => x.Token)
            .IsRequired();
        builder.Property(x => x.ExpirationTime)
            .IsRequired();
        builder.HasKey(x => x.Token);
        builder.ToTable("RevokedRefreshTokens");
    }
}