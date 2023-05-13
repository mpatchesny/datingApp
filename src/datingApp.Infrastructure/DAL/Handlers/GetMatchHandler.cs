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
        var match = await _dbContext.Matches
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == query.MatchId);
        
        return new MatchDto
            {
                Id = match.Id,
                User = await _dbContext.Users
                        .AsNoTracking()
                        .Where(u => u.Id == ((match.UserId1 == query.UserId) ? match.UserId2 : match.UserId1))
                        .Include(u => u.Photos)
                        .Select(u => u.AsPublicDto(0))
                        .FirstOrDefaultAsync(),
                IsDisplayed = ((match.UserId1 == query.UserId) ? match.IsDisplayedByUser1 : match.IsDisplayedByUser2),
                // FIXME: magic string
                Messages = _dbContext.Messages.Where(m => m.MatchId == match.Id).OrderByDescending(m => m.CreatedAt).Take(10).Select(x => x.AsDto()),
                CreatedAt = match.CreatedAt
            }; 
    }
}