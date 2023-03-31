using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Core.Entities;

namespace datingApp.Infrastructure.DAL.Handlers;

public static class Extensions
{
    public static PublicUserDto AsDto(this User entity)
    {
        return new()
        {
            Id = entity.Id,
            Age = entity.GetAge(),
            Bio = entity.Bio,
            Distance = 0, // FIXME
            Job = entity.Job,
            Name = entity.Name,
            Sex = (int) entity.Sex,
            Photos = entity.Photos.Select(x => x.AsDto()).ToList()
        };
    }

    public static PhotoDto AsDto(this Photo entity)
    {
        return new()
        {
            Id = entity.Id,
            Oridinal = entity.Oridinal,
            Path = entity.Path,
            UserId = entity.UserId
        };
    }

    public static MessageDto AsDto(this Message entity)
    {
        return new()
        {
            Id = entity.Id,
            MatchId = entity.MatchId,
            SendFromId = entity.SendFromId,
            SendToId = entity.SendToId,
            Text = entity.Text,
            IsDisplayed = entity.IsDisplayed,
            CreatedAt = entity.CreatedAt
        };
    }

    public static MatchDto AsDto(this Match entity)
    {
        return new()
        {
            Id = entity.Id,
            UserId1 = entity.UserId1,
            UserId2 = entity.UserId2,
            CreatedAt = entity.CreatedAt,
            Messages = entity.Messages.Select(x => x.AsDto()).ToList()
        };
    }

    public static UserSettingsDto AsDto(this UserSettings entity)
    {
        return new()
        {
            UserId = entity.UserId,
            DiscoverAgeFrom = entity.DiscoverAgeFrom,
            DiscoverAgeTo = entity.DiscoverAgeTo,
            DiscoverRange = entity.DiscoverRange,
            DiscoverSex = (int) entity.DiscoverSex
        };
    }
}