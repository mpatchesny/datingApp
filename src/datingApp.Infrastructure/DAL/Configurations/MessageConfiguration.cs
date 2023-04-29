using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .IsRequired();
        builder.Property(x => x.MatchId)
            .IsRequired();
        builder.Property(x => x.SendFromId)
            .IsRequired();
        builder.Property(x => x.IsDisplayed)
            .IsRequired();
        builder.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.SendFromId);

        builder.HasIndex(x => new {x.MatchId, x.CreatedAt});
    }
}