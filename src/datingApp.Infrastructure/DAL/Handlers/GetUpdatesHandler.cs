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

        var newMatches = _dbContext.Matches
                        .AsNoTracking()
                        .Where(x => usersMatches.Contains(x.Id))
                        .Where(x => x.CreatedAt >= query.LastActivityTime)
                        .Select(x => x.Id);

        var newMatchesAndNewMessages = newMatches.Union(newMessages);

        return await _dbContext.Matches
                        .AsNoTracking()
                        .Where(x => newMatchesAndNewMessages.Contains(x.Id))
                        .Select(x => new
                            {
                                Match = x,
                                User = _dbContext.Users.Where(u => u.Id == ((x.UserId1 != query.UserId) ? x.UserId1 : x.UserId2)).FirstOrDefault(),
                                Photo = _dbContext.Photos.Where(p => p.UserId == ((x.UserId1 != query.UserId) ? x.UserId1 : x.UserId2))
                                            .FirstOrDefault(p => p.Oridinal == 0)
                            })
                        .Select(x =>
                            new MatchDto()
                            {
                                Id = x.Match.Id,
                                UserId = x.User.Id,
                                Name = x.User.Name,
                                IsDisplayed = ((x.Match.UserId1 == query.UserId) ? x.Match.IsDisplayedByUser1 : x.Match.IsDisplayedByUser2),
                                ProfilePicture = x.Photo == null ? null : x.Photo.AsDto(),
                                Messages = x.Match.Messages.OrderByDescending(m => m.CreatedAt).Take(1).Select(x => x.AsDto()).ToList(),
                                CreatedAt = x.Match.CreatedAt
                            })
                        .ToListAsync();
    }
}