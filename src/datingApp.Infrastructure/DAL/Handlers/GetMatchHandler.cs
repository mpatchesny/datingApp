using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
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
        var swipeExists = await _dbContext.Swipes
                                .AsNoTracking()
                                .AnyAsync(x => 
                                    (x.SwipedById == query.SwipedById && x.SwipedWhoId == query.SwipedWhoId && x.Like == Like.Like) &&
                                    (x.SwipedById == query.SwipedWhoId && x.SwipedWhoId == query.SwipedById && x.Like == Like.Like));
        return new IsMatchDto {
            Match = swipeExists
        };
    }
}