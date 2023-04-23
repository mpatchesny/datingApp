using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMatchHandler : IQueryHandler<GetMatch, IsMatchDto>
{
    private readonly DatingAppDbContext _dbContext;
    public GetMatchHandler(DatingAppDbContext dbContext = null)
    {
        _dbContext = dbContext;
    }

    public async Task<IsMatchDto> HandleAsync(GetMatch query)
    {
        var swipes = await _dbContext.Swipes
                                    .AsNoTracking()
                                    .Where(x => x.SwippedById == query.SwipedById || x.SwippedById == query.SwipedWhoId)
                                    .Where(x => x.SwippedWhoId == query.SwipedWhoId || x.SwippedWhoId == query.SwipedById)
                                    .Where(x => x.Like == Core.Entities.Like.Like)
                                    .Select(x => new { x.SwippedById, x.SwippedWhoId })
                                    .Distinct()
                                    .ToListAsync();
        return new IsMatchDto {
            Match = (swipes.Count() == 2) ? true : false
        };
    }
}