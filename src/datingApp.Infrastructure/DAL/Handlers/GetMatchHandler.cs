using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMatchHandler : IQueryHandler<GetMatch, MatchDto>
{
    private readonly DatingAppDbContext _dbContext;
    public GetMatchHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MatchDto> HandleAsync(GetMatch query)
    {
        var dbQuery = 
            from match in _dbContext.Matches.Include(m => m.Messages)
            from user in _dbContext.Users.Include(u => u.Photos)
            where (user.Id == match.UserId1 || user.Id == match.UserId2) && user.Id != query.UserId
            where match.Id == query.MatchId
            select new 
            {
                Match = match,
                User = user
            };

        var data = await dbQuery.FirstOrDefaultAsync();
        if (data == null) return null;

        return new MatchDto
            {
                Id = data.Match.Id,
                User = data.User.AsPublicDto(0),
                IsDisplayed = (data.Match.UserId1 == query.UserId) ? data.Match.IsDisplayedByUser1 : data.Match.IsDisplayedByUser2,
                // FIXME: magic string
                Messages = data.Match.Messages.OrderByDescending(m => m.CreatedAt).Take(10).OrderBy(m => m.CreatedAt).Select(m => m.AsDto()),
                CreatedAt = data.Match.CreatedAt
            }; 
    }
}