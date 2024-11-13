using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Infrastructure.DAL.Models;

namespace datingApp.Infrastructure.DAL.Models;

internal static class Extensions
{
    public static MatchDto AsDto(this MatchReadModel readModel)
    {
        var dto = new MatchDto()
        {
            Id = readModel.Id,
            User = readModel.User.AsPublicDto(),
            IsDisplayed = readModel.IsDisplayed,
            Messages = MessagesAsDto(readModel.Messages, readModel),
            CreatedAt = readModel.CreatedAt
        };

        return dto;
    }

    public static PublicUserDto AsPublicDto(this UserReadModel readModel, int distance = 0)
    {
        return new()
        {
            Id = readModel.Id,
            Name = readModel.Name,
            Age = readModel.Age,
            Sex = (int) readModel.Sex,
            DistanceInKms = distance,
            Job = readModel.Job,
            Bio = readModel.Bio,
            Photos = PhotosAsDto(readModel)
        };
    }

    public static PrivateUserDto AsPrivateDto(this UserReadModel readModel)
    {
        return new()
        {
            Id = readModel.Id,
            Age = readModel.Age,
            DateOfBirth = readModel.DateOfBirth,
            Bio = readModel.Bio,
            Job = readModel.Job,
            Name = readModel.Name,
            Sex = readModel.Sex,
            Photos = PhotosAsDto(readModel),
            Settings = new UserSettingsDto
            {
                UserId = readModel.Id,
                PreferredSex = readModel.Settings.PreferredSex,
                PreferredAgeFrom  = readModel.Settings.PreferredAgeFrom,
                PreferredAgeTo  = readModel.Settings.PreferredAgeTo,
                PreferredMaxDistance  = readModel.Settings.PreferredMaxDistance,
                Lat = readModel.Settings.Lat,
                Lon = readModel.Settings.Lon
            }
        };
    }

    public static MessageDto AsDto(this MessageReadModel readModel)
    {
        return new ()
        {
            Id = readModel.Id,
            MatchId = readModel.Match.Id,
            SendFromId = readModel.SendFrom.Id,
            Text = readModel.Text,
            IsDisplayed = readModel.IsDisplayed,
            CreatedAt = readModel.CreatedAt
        };
    }

    public static PhotoDto AsDto(this PhotoReadModel readModel)
    {
        return new()
        {
            Id = readModel.Id,
            Oridinal = readModel.Oridinal,
            Url = readModel.Url,
            UserId = readModel.User.Id
        };
    }

    private static List<PhotoDto> PhotosAsDto(this UserReadModel user)
    {
        var photos = new List<PhotoDto>();
        foreach (var photo in user.Photos.OrderBy(photo => photo.Oridinal))
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

    private static List<MessageDto> MessagesAsDto(this IEnumerable<MessageReadModel> messages, MatchReadModel match)
    {
        var messagesDto = new List<MessageDto>();
        foreach (var message in messages.OrderBy(message => message.CreatedAt))
        {
            messagesDto.Add(new MessageDto
            {
                Id = message.Id,
                MatchId = match.Id,
                SendFromId = message.SendFrom.Id,
                Text = message.Text,
                IsDisplayed = message.IsDisplayed,
                CreatedAt = message.CreatedAt
            });
        }
        return messagesDto;
    }
}