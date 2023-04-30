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

    public async Task<IEnumerable<MatchDto>> HandleAsync(GetUpdates query)
    {
        var usersMatches = _dbContext.Matches
                        .AsNoTracking()
                        .Where(x => x.UserId1 == query.UserId || x.UserId2 == query.UserId)
                        .Select(x => x.Id);

        var newMessages = _dbContext.Messages
                        .AsNoTracking()
                        .Where(x => usersMatches.Contains(x.MatchId))
                        .Where(x => x.CreatedAt >= query.LastActivityTime)
                        .Select(x => x.MatchId);

        var newMatchesAndNewMessages = await _dbContext.Matches
                        .AsNoTracking()
                        .Where(x => usersMatches.Contains(x.Id))
                        .Where(x => x.CreatedAt >= query.LastActivityTime)
                        .Select(x => x.Id)
                        .Union(newMessages)
                        .ToListAsync();

        var dbQuery = 
            from m in _dbContext.Matches
            from u in _dbContext.Users
            from p in _dbContext.Photos.Where(p => p.Oridinal == 0 && p.UserId == u.Id).DefaultIfEmpty()
            where u.Id == m.UserId1 || u.Id == m.UserId2
            where m.UserId1 == query.UserId || m.UserId2 == query.UserId 
            where u.Id != query.UserId
            where newMatchesAndNewMessages.Contains(m.Id)
            select new 
            {
                Match = m,
                User = u,
                Photo = p
            };

        var messagesList = await _dbContext.Messages
                        .AsNoTracking()
                        .Where(x => newMessages.Contains(x.Id))
                        .ToListAsync(); 

        var data = await dbQuery.AsNoTracking().ToListAsync();

        List<MatchDto> dataDto = new List<MatchDto>();
        foreach (var x in data)
        {
            dataDto.Add(
                new MatchDto()
                {
                    Id = x.Match.Id,
                    UserId = x.User.Id,
                    Name = x.User.Name,
                    IsDisplayed = ((x.Match.UserId1 == query.UserId) ? x.Match.IsDisplayedByUser1 : x.Match.IsDisplayedByUser2),
                    ProfilePicture = x.Photo == null ? null : x.Photo.AsDto(),
                    Messages = messagesList.Where(m => m.MatchId == x.Match.Id).Select(x => x.AsDto()).ToList(),
                    CreatedAt = x.Match.CreatedAt
                });
        }

        return dataDto;
    }
}