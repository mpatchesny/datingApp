using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMatchesHandler : IQueryHandler<GetMatches, IEnumerable<MatchDto>>
{
    private readonly DatingAppDbContext _dbContext;
    public GetMatchesHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<MatchDto>> HandleAsync(GetMatches query)
    {
        return await _dbContext.Matches
                        .AsNoTracking()
                        .Where(x => x.UserId1 == query.UserId || x.UserId2 == query.UserId)
                        .Include(x => x.Messages
                                .OrderByDescending(message => message.CreatedAt)
                                .Take(1))
                        .Select(x => new
                            {
                                Match = x,
                                User = _dbContext.Users.Where(u => u.Id == ((x.UserId1 != query.UserId) ? x.UserId1 : x.UserId2)).FirstOrDefault(),
                                Photo = _dbContext.Photos.Where(p => p.Id == ((x.UserId1 != query.UserId) ? x.UserId1 : x.UserId2))
                                            .FirstOrDefault(p => p.Oridinal == 0)
                            })
                        .Select(x =>
                            new MatchDto()
                            {
                                Id = x.Match.Id,
                                UserId = x.User.Id,
                                Name = x.User.Name,
                                IsDisplayed = ((x.Match.UserId1 == query.UserId) ? x.Match.IsDisplayedByUser1 : x.Match.IsDisplayedByUser2),
                                ProfilePicture = (x.Photo != null) ? x.Photo.AsDto() : null,
                                Messages = x.Match.Messages.Select(x => x.AsDto()).ToList(),
                                CreatedAt = DateTime.UtcNow
                            })
                        .Skip((query.Page - 1) * query.PageSize)
                        .Take(query.PageSize)
                        .ToListAsync();
    }
}