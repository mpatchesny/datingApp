using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Spatial;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetUpdatesHandler : IQueryHandler<GetUpdates, IEnumerable<MatchDto>>
{
    private readonly DatingAppDbContext _dbContext;
    private readonly ISpatial _spatial;

    public GetUpdatesHandler(DatingAppDbContext dbContext, ISpatial spatial)
    {
        _dbContext = dbContext;
        _spatial = spatial;
    }

    private async Task<IEnumerable<Guid>> GetMatchesWithMessagesPastActivityTimeAsync(Guid userId, DateTime lastActivityTime)
    {
        return await _dbContext.Matches
            .AsNoTracking()
            .Where(match => match.UserId1.Equals(userId) || match.UserId2.Equals(userId))
            .Where(match => match.Messages.Any(message => message.CreatedAt >= lastActivityTime))
            .Select(match => match.Id.Value)
            .ToListAsync();
    }

    private async Task<IEnumerable<Guid>> GetMatchesPastActivityTimeAsync(Guid userId, DateTime lastActivityTime)
    {
        return await _dbContext.Matches
            .AsNoTracking()
            .Where(match => match.UserId1.Equals(userId) || match.UserId2.Equals(userId))
            .Where(match => match.CreatedAt >= lastActivityTime)
            .Select(match => match.Id.Value)
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchDto>> HandleAsync(GetUpdates query)
    {
        var requestedBy = await _dbContext.Users
            .AsNoTracking()
            .Include(user => user.Settings)
            .FirstOrDefaultAsync(user => user.Id.Equals(query.UserId));

        if (requestedBy == null)
        {
            throw new UserNotExistsException(query.UserId);
        }

        var newMessagesMatchId = await GetMatchesWithMessagesPastActivityTimeAsync(query.UserId, query.LastActivityTime);
        var newMatchesId = await GetMatchesPastActivityTimeAsync(query.UserId, query.LastActivityTime);
        var newMessagesAndMatches = newMessagesMatchId.Union(newMatchesId);

        var dbQuery = 
            from match in _dbContext.Matches
                .Include(match => match.Messages
                    .Where(message => message.CreatedAt >= query.LastActivityTime))
            from user in _dbContext.Users
                .Include(user => user.Photos)
                .Include(user => user.Settings)
            where !user.Id.Equals(query.UserId)
            where match.UserId1.Equals(user.Id) || match.UserId2.Equals(user.Id)
            where newMessagesAndMatches.Contains(match.Id)
            select new 
            {
                Match = match,
                User = user
            };

        var data = await dbQuery.AsNoTracking().ToListAsync();

        var dataDto = new List<MatchDto>();
        foreach (var item in data)
        {
            var distanceInKms = _spatial.CalculateDistanceInKms(requestedBy.Settings.Location.Lat, requestedBy.Settings.Location.Lon, 
                item.User.Settings.Location.Lat, item.User.Settings.Location.Lon);

            dataDto.Add(
                new MatchDto()
                {
                    Id = item.Match.Id,
                    User = item.User.AsPublicDto(distanceInKms),
                    IsDisplayed = item.Match.IsDisplayedByUser(query.UserId),
                    Messages =  item.Match.MessagesAsDto(),
                    CreatedAt = item.Match.CreatedAt
                });
        }

        return dataDto;
    }
}