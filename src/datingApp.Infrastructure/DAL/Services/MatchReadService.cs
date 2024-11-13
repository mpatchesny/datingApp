using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace datingApp.Infrastructure.DAL.Services;

internal sealed class MatchReadService
{
    private readonly DatingAppDbContext _dbContext;

    public MatchReadService(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<MatchReadModel>> GetByUserIdAsync(Guid userWhoRequestedId, int matchLimit, int matchOffset, int messageLimit, int messageOffset)
    {
        var query = 
            from match in _dbContext.Matches
                .Include(match => match.Messages
                    .OrderByDescending(message => message.CreatedAt)
                    .Skip(messageOffset)
                    .Take(messageLimit)
                    .OrderBy(message => message.CreatedAt))
            from user in _dbContext.Users.Include(user => user.Photos).Include(user => user.Settings)
            where !user.Id.Equals(userWhoRequestedId)
            where match.UserId1.Equals(user.Id) || match.UserId2.Equals(user.Id)
            where match.UserId1.Equals(userWhoRequestedId) || match.UserId2.Equals(userWhoRequestedId)
            select new 
            {
                Match = match,
                User = user
            };

        var data = await query
            .AsNoTracking()
            .OrderByDescending(item => item.Match.CreatedAt)
            .Skip(matchOffset)
            .Take(matchLimit)
            .ToListAsync();

        return MatchUserListToMatchReadModel(data, userWhoRequestedId);
    }

    public async Task<int> CountByUserIdAsync(Guid userRequestedId)
    {
        return await _dbContext.Matches
            .Where(match => match.UserId1.Equals(userRequestedId) || match.UserId2.Equals(userRequestedId))
            .SelectMany(match => _dbContext.Users
                .Where(user => match.UserId1.Equals(user.Id) || match.UserId2.Equals(user.Id))
                .Where(user => !user.Id.Equals(userRequestedId))
                .Select(user => 1))
            .CountAsync();
    }

    private static List<MatchReadModel> MatchUserListToMatchReadModel(IEnumerable<dynamic> data, Guid userWhoRequestedId)
    {
        var matchesReadModel = new List<MatchReadModel>();
        foreach (var item in data.OrderBy(item => item.Match.CreatedAt))
        {
            var userReadModel = UserToUserReadModel(item.user);
            matchesReadModel.Add(MatchToMatchReadModel(item.match, userReadModel, userWhoRequestedId));
        }
        return matchesReadModel;
    }

    private static MatchReadModel MatchToMatchReadModel(Match match, UserReadModel user, Guid userWhoRequestedId)
    {
        var matchReadModel = new MatchReadModel()
        {
            Id = match.Id,
            // User = user,
            IsDisplayed = match.UserId1.Value == userWhoRequestedId ? match.IsDisplayedByUser1 : match.IsDisplayedByUser2,
            CreatedAt = match.CreatedAt,
            Messages = new List<MessageReadModel>()
        };

        matchReadModel.Messages = match.Messages
            .OrderBy(message => message.CreatedAt)
            .Select(message => new MessageReadModel
            {
                Id = message.Id,
                IsDisplayed = message.IsDisplayed,
                // SendFromId = message.SendFromId,
                Text = message.Text.Value,
                CreatedAt = message.CreatedAt,
                Match = matchReadModel
            })
            .ToList();

        return matchReadModel;
    }

    private static List<PhotoReadModel> PhotosToListOfPhotosReadModel(ICollection<Photo> photos, UserReadModel user)
    {
        return photos
            .OrderBy(photo => photo.Oridinal)
            .Select(photo => new PhotoReadModel
            {
                Id = photo.Id,
                Url = photo.Url,
                Oridinal = photo.Oridinal
                // User = user
            })
            .ToList();
    }

    private static UserReadModel UserToUserReadModel(User user)
    {
        var userReadModel = new UserReadModel()
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            Email = user.Email,
            Age = user.GetAge(),
            DateOfBirth = user.DateOfBirth,
            Sex = (int) user.Sex,
            Job = user.Job,
            Bio = user.Bio,
            Settings = new UserSettingsReadModel()
            {
                PreferredAgeFrom = user.Settings.PreferredAge.From,
                PreferredAgeTo = user.Settings.PreferredAge.To,
                PreferredMaxDistance = user.Settings.PreferredMaxDistance,
                PreferredSex = (int) user.Settings.PreferredSex,
                Lat = user.Settings.Location.Lat,
                Lon = user.Settings.Location.Lon
            }
        };

        // userReadModel.Settings.User = userReadModel;
        userReadModel.Photos = PhotosToListOfPhotosReadModel(user.Photos, userReadModel);
        return userReadModel;
    }
}