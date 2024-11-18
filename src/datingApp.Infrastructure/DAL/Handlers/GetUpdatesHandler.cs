using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetUpdatesHandler : IQueryHandler<GetUpdates, IEnumerable<MatchDto>>
{
    private readonly DatingAppDbContext _dbContext;
    public GetUpdatesHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private async Task<IEnumerable<Guid>> GetMatchesByMessagesPastGivenActivityTimeAsync(Guid userId, DateTime lastActivityTime)
    {
        return await _dbContext.Matches
                    .Where(match => match.UserId1.Equals(userId) || match.UserId2.Equals(userId))
                    .Where(match => match.Messages.Any(message => message.CreatedAt >= lastActivityTime))
                    .AsNoTracking()
                    .Select(match => match.Id.Value)
                    .ToListAsync();
    }

    private async Task<IEnumerable<Guid>> GetMatchesPastGivenActivityTimeAsync(Guid userId, DateTime lastActivityTime)
    {
        return await _dbContext.Matches
                    .Where(match => match.UserId1.Equals(userId) || match.UserId2.Equals(userId))
                    .Where(match => match.CreatedAt >= lastActivityTime)
                    .AsNoTracking()
                    .Select(match => match.Id.Value)
                    .ToListAsync();
    }

    public async Task<IEnumerable<MatchDto>> HandleAsync(GetUpdates query)
    {
        if (!await _dbContext.Users.AnyAsync(x => x.Id.Equals(query.UserId)))
        {
            throw new UserNotExistsException(query.UserId);
        }

        var newMessagesMatchId = await GetMatchesByMessagesPastGivenActivityTimeAsync(query.UserId, query.LastActivityTime);
        var newMatchesId = await GetMatchesPastGivenActivityTimeAsync(query.UserId, query.LastActivityTime);
        var newMessagesAndMatches = newMessagesMatchId.Union(newMatchesId);

        var dbQuery = 
            from match in _dbContext.Matches
                .Include(match => match.Messages
                    .Where(message => message.CreatedAt >= query.LastActivityTime))
            from user in _dbContext.Users.Include(user => user.Photos).Include(user => user.Settings)
            where (match.UserId1.Equals(user.Id) || match.UserId2.Equals(user.Id)) && !user.Id.Equals(query.UserId)
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
            dataDto.Add(
                new MatchDto()
                {
                    Id = item.Match.Id,
                    User = item.User.AsPublicDto(0),
                    IsDisplayed = item.Match.IsDisplayedByUser(query.UserId),
                    Messages =  item.Match.MessagesAsDto(),
                    CreatedAt = item.Match.CreatedAt
                });
        }

        return dataDto;
    }
}