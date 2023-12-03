using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
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

    private IQueryable<Guid> GetMatchesIdByUserId(Guid userId)
    {
        return _dbContext.Matches
                        .AsNoTracking()
                        .Where(x => x.UserId1 == userId || x.UserId2 == userId)
                        .Select(x => x.Id);
    }

    private IQueryable<Guid> GetMessagesPastGivenActivityTime(IQueryable<Guid> usersMatches, DateTime lastActivityTime)
    {
        return _dbContext.Messages
                        .AsNoTracking()
                        .Where(x => usersMatches.Contains(x.MatchId))
                        .Where(x => x.CreatedAt >= lastActivityTime)
                        .Select(x => x.MatchId);
    }

    private IQueryable<Guid> GetMatchesPastGivenActivityTime(IQueryable<Guid> usersMatches, DateTime lastActivityTime)
    {
        return  _dbContext.Matches
                        .AsNoTracking()
                        .Where(x => usersMatches.Contains(x.Id))
                        .Where(x => x.CreatedAt >= lastActivityTime)
                        .Select(x => x.Id);
    }

    public async Task<IEnumerable<MatchDto>> HandleAsync(GetUpdates query)
    {
        var usersMatches = GetMatchesIdByUserId(query.UserId);
        var newMessagesMatchId = GetMessagesPastGivenActivityTime(usersMatches, query.LastActivityTime);
        var newMatchesId = GetMatchesPastGivenActivityTime(usersMatches, query.LastActivityTime);
        var newMessagesAndMatches = newMessagesMatchId.Union(newMatchesId);

        var dbQuery = 
            from m in _dbContext.Matches
            from u in _dbContext.Users
            where u.Id == m.UserId1 || u.Id == m.UserId2
            where m.UserId1 == query.UserId || m.UserId2 == query.UserId 
            where u.Id != query.UserId
            where newMessagesAndMatches.Contains(m.Id)
            select new 
            {
                Match = m,
                User = u,
            };

        var messagesList = await _dbContext.Messages
                        .AsNoTracking()
                        .Where(x => newMessagesMatchId.Contains(x.MatchId))
                        .Where(x => x.CreatedAt >= query.LastActivityTime)
                        .OrderBy(x => x.CreatedAt)
                        .ToListAsync(); 

        var data = await dbQuery.AsNoTracking().ToListAsync();

        List<MatchDto> dataDto = new List<MatchDto>();
        foreach (var x in data)
        {
            dataDto.Add(
                new MatchDto()
                {
                    Id = x.Match.Id,
                    User = await _dbContext.Users
                        .AsNoTracking()
                        .Where(u => u.Id == ((x.Match.UserId1 == query.UserId) ? x.Match.UserId2 : x.Match.UserId1))
                        .Include(u => u.Photos)
                        .Select(u => u.AsPublicDto(0))
                        .FirstOrDefaultAsync(),
                    IsDisplayed = (x.Match.UserId1 == query.UserId) ? x.Match.IsDisplayedByUser1 : x.Match.IsDisplayedByUser2,
                    Messages = messagesList.Where(m => m.MatchId == x.Match.Id).Select(x => x.AsDto()).ToList(),
                    CreatedAt = x.Match.CreatedAt
                });
        }

        return dataDto;
    }
}