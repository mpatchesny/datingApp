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
                        .Include(match => match.Messages
                                .OrderByDescending(message => message.CreatedAt)
                                .Take(1))
                        .Skip((query.Page - 1) * query.PageSize)
                        .Take(query.PageSize)
                        .Select(x => x.AsDto())
                        .ToListAsync();
    }
}