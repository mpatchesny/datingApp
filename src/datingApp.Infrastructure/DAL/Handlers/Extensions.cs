using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Spatial;
using datingApp.Core.Entities;

namespace datingApp.Infrastructure.DAL.Handlers;

internal static class Extensions
{
    public static PublicUserDto AsPublicDto(this User entity, int distanceInKms)
    {
        return new()
        {
            Id = entity.Id,
            Age = entity.GetAge(),
            DistanceInKms = distanceInKms,
            Bio = entity.Bio,
            Job = entity.Job,
            Name = entity.Name,
            Sex = (int) entity.Sex,
            Photos = PhotosAsDto(entity)
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
            Photos = PhotosAsDto(entity)
        };
    }

    public static UserSettingsDto AsDto(this UserSettings entity)
    {
        return new()
        {
            UserId = entity.UserId,
            PreferredSex = (int) entity.PreferredSex,
            PreferredAgeFrom = entity.PreferredAge.From,
            PreferredAgeTo = entity.PreferredAge.To,
            PreferredMaxDistance = entity.PreferredMaxDistance,
            Lat = entity.Location.Lat,
            Lon = entity.Location.Lon
        };
    }

    public static MatchDto AsDto(this Match entity, Guid asSeenByUserId, int distanceInKms)
    {
        var user1 = entity.Users.ElementAt(0);
        var user2 = entity.Users.ElementAt(1);

        var userDto = user1.Id.Equals(asSeenByUserId) ? 
            user2.AsPublicDto(distanceInKms) :
            user1.AsPublicDto(distanceInKms);

        return new ()
        {
            Id = entity.Id,
            User = userDto,
            IsDisplayed = entity.IsDisplayedByUser(asSeenByUserId),
            Messages =  entity.MessagesAsDto(),
            CreatedAt = entity.CreatedAt
        };
    }

    public static List<MessageDto> MessagesAsDto(this Match match)
    {
        var messages = new List<MessageDto>();
        foreach (var message in match.Messages)
        {
            messages.Add(new MessageDto
            {
                Id = message.Id,
                MatchId = match.Id,
                SendFromId = message.SendFromId,
                Text = message.Text,
                IsDisplayed = message.IsDisplayed,
                CreatedAt = message.CreatedAt
            });
        }
        return messages;
    }

    public static List<PhotoDto> PhotosAsDto(this User user)
    {
        var photos = new List<PhotoDto>();
        foreach (var photo in user.Photos.OrderBy(p => p.Oridinal))
        {
            photos.Add(new PhotoDto
            {
                Id = photo.Id,
                Oridinal = photo.Oridinal,
                Url = photo.Url,
                UserId = user.Id
            });
        }
        return photos;
    }
}