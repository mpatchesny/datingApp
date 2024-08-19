using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
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
        if (!await _dbContext.Matches.AnyAsync(x => x.Id == query.MatchId))
        {
            throw new MatchNotExistsException(query.MatchId);
        }

        var dbQuery = _dbContext.Messages
                            .AsNoTracking()
                            .Where(x => x.MatchId == query.MatchId)
                            .OrderByDescending(x => x.CreatedAt);

        var data = await dbQuery
                        .Skip((query.Page - 1) * query.PageSize)
                        .Take(query.PageSize)
                        .Select(x => x.AsDto())
                        .ToListAsync();

        data = data.OrderBy(x => x.CreatedAt).ToList();

        var pageCount = (int) (dbQuery.Count() + query.PageSize - 1) / query.PageSize;

        return new PaginatedDataDto{
            Page = query.Page,
            PageSize = query.PageSize,
            PageCount = pageCount,
            Data = new List<dynamic>(data)
        };
    }
}