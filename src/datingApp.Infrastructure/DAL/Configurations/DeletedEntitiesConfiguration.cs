using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class DeletedEntitiesConfiguration : IEntityTypeConfiguration<DeletedEntityDto>
{
    public void Configure(EntityTypeBuilder<DeletedEntityDto> builder)
    {
        builder.Property(x => x.Id)
            .IsRequired();
        builder.HasIndex(x => new {x.Id});
    }
}