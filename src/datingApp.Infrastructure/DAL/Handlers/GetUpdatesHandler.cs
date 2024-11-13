using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Infrastructure.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetUpdatesHandler : IQueryHandler<GetUpdates, IEnumerable<MatchDto>>
{
    private readonly DatingAppDbContext _dbContext;
    private readonly DatingAppReadDbContext _readDbContext;
    public GetUpdatesHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private async Task<IEnumerable<Guid>> GetMatchesAndMessagesPastGivenActivityTimeAsync(Guid userId, DateTime lastActivityTime)
    {
        return await _dbContext.Matches
                    .AsNoTracking()
                    .Where(match => match.UserId1.Equals(userId) || match.UserId2.Equals(userId))
                    .Where(match => match.CreatedAt >= lastActivityTime || 
                        match.Messages.Any(message => message.CreatedAt >= lastActivityTime))
                    .Select(match => match.Id.Value)
                    .ToListAsync();
    }

    public async Task<IEnumerable<MatchDto>> HandleAsync(GetUpdates query)
    {
        if (!await _dbContext.Users.AnyAsync(x => x.Id.Equals(query.UserId)))
        {
            throw new UserNotExistsException(query.UserId);
        }

        var newMatchesAndMessages = await GetMatchesAndMessagesPastGivenActivityTimeAsync(query.UserId, query.LastActivityTime);

        var altQuery = await _readDbContext.Users
            .Where(user => user.Id == query.UserId)
            .Include(user => user.Matches)
            .ThenInclude(match => match.Messages
                .Where(message => message.CreatedAt >= query.LastActivityTime))
            .Include(user => user.Matches)
            .ThenInclude(match => match.LastChangeTime >= query.LastActivityTime)
            // .OrderByDescending(match => match.LastChangeTime)
            .SelectMany(user => user.Matches)
            .Select(match => match.AsDto())
            .ToListAsync();

        var dbQuery = 
            from match in _dbContext.Matches
                .Include(match => match.Messages
                    .Where(message => message.CreatedAt >= query.LastActivityTime))
            from user in _dbContext.Users.Include(user => user.Photos).Include(user => user.Settings)
            where match.UserId1.Equals(user.Id) || match.UserId2.Equals(user.Id)
            where !user.Id.Equals(query.UserId)
            where newMatchesAndMessages.Contains(match.Id)
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
                    IsDisplayed = item.Match.UserId1.Equals(query.UserId) ? item.Match.IsDisplayedByUser1 : item.Match.IsDisplayedByUser2,
                    Messages =  item.Match.MessagesAsDto(),
                    CreatedAt = item.Match.CreatedAt
                });
        }

        return dataDto;
    }
}