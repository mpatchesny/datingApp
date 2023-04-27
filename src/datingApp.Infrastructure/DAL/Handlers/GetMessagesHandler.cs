using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMessagesHandler : IQueryHandler<GetMessages, PaginatedDataDto>
{
    private readonly DatingAppDbContext _dbContext;
    public GetMessagesHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedDataDto> HandleAsync(GetMessages query)
    {
        var data = await _dbContext.Messages
                            .AsNoTracking()
                            .Where(x => x.MatchId == query.MatchId)
                            .OrderByDescending(x => x.CreatedAt)
                            .Skip((query.Page - 1) * query.PageSize)
                            .Take(query.PageSize)
                            .Select(x => x.AsDto())
                            .ToListAsync();

        return new PaginatedDataDto(
            Page = query.Page,
            PageSize = query.PageSize,
            PageCount = 0,
            Data = data
            );
    }
}