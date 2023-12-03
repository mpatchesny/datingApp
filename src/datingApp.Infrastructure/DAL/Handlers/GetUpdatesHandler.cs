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
            from match in _dbContext.Matches.Include(m => m.Messages)
            from user in _dbContext.Users.Include(u => u.Photos)
            where (user.Id == match.UserId1 || user.Id == match.UserId2) && user.Id != query.UserId
            where newMessagesAndMatches.Contains(match.Id)
            select new 
            {
                Match = match,
                User = user
            };

        var data = await dbQuery.AsNoTracking().ToListAsync();

        List<MatchDto> dataDto = new List<MatchDto>();
        foreach (var x in data)
        {
            dataDto.Add(
                new MatchDto()
                {
                    Id = x.Match.Id,
                    User = x.User.AsPublicDto(0),
                    IsDisplayed = (x.Match.UserId1 == query.UserId) ? x.Match.IsDisplayedByUser1 : x.Match.IsDisplayedByUser2,
                    Messages =  x.Match.Messages.Where(m => m.CreatedAt >= query.LastActivityTime).OrderBy(m => m.CreatedAt).Select(x => x.AsDto()).ToList(),
                    CreatedAt = x.Match.CreatedAt
                });
        }

        return dataDto;
    }
}