using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Core.Entities;

namespace datingApp.Infrastructure.DAL.Handlers;

internal static class Extensions
{
    public static PublicUserDto AsPublicDto(this User entity, int distance)
    {
        return new()
        {
            Id = entity.Id,
            Age = entity.GetAge(),
            DistanceInKms = distance,
            Bio = entity.Bio,
            Job = entity.Job,
            Name = entity.Name,
            Sex = (int) entity.Sex,
            Photos = entity.Photos.Select(x => x.AsDto()).ToList()
        };
    }

    public static PrivateUserDto AsPrivateDto(this User entity)
    {
        return new()
        {
            Id = entity.Id,
            Age = entity.GetAge(),
            DateOfBirth = entity.DateOfBirth,
            Email = entity.Email,
            Phone = entity.Phone,
            Bio = entity.Bio,
            Job = entity.Job,
            Name = entity.Name,
            Sex = (int) entity.Sex,
            Settings = entity.Settings.AsDto(),
            Photos = entity.Photos.Select(x => x.AsDto()).ToList()
        };
    }

    public static PhotoDto AsDto(this Photo entity)
    {
        return new()
        {
            Id = entity.Id,
            Oridinal = entity.Oridinal,
            Url = entity.Url,
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
            Text = entity.Text,
            IsDisplayed = entity.IsDisplayed,
            CreatedAt = entity.CreatedAt
        };
    }

    public static UserSettingsDto AsDto(this UserSettings entity)
    {
        return new()
        {
            UserId = entity.UserId,
            DiscoverSex = (int) entity.PreferredSex,
            DiscoverAgeFrom = entity.DiscoverAgeFrom,
            DiscoverAgeTo = entity.DiscoverAgeTo,
            DiscoverRange = entity.DiscoverRange,
            Lat = entity.Lat,
            Lon = entity.Lon
        };
    }
}