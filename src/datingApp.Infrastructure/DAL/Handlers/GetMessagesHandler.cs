using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMessagesHandler : IQueryHandler<GetMessages, IEnumerable<MessageDto>>
{
    private readonly DatingAppDbContext _dbContext;
    public GetMessagesHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<MessageDto>> HandleAsync(GetMessages query)
    {
        return await _dbContext.Messages
                            .AsNoTracking()
                            .Where(x => x.MatchId == query.MatchId)
                            .Select(x => x.AsDto())
                            .OrderByDescending(x => x.CreatedAt)
                            .Skip((query.Page - 1) * query.PageSize)
                            .Take(query.PageSize)
                            .ToListAsync();
    }
}