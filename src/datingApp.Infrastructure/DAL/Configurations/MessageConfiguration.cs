using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace datingApp.Infrastructure.DAL.Configurations;

internal sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new MessageId(x))
            .IsRequired();
        builder.Property(x => x.MatchId)
            .HasConversion(x => x.Value, x => new MatchId(x))
            .IsRequired();
        builder.Property(x => x.SendFromId)
            .HasConversion(x => x.Value, x => new UserId(x))
            .IsRequired();
        builder.Property(x => x.IsDisplayed)
            .IsRequired();
        builder.Property(x => x.Text)
            .HasConversion(x => x.Value, x => new MessageText(x))
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