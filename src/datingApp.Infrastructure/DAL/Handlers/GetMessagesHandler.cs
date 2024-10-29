using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMessagesHandler : IQueryHandler<GetMessages, PaginatedDataDto>
{
    private readonly DatingAppDbContext _dbContext;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public GetMessagesHandler(DatingAppDbContext dbContext, IDatingAppAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _authorizationService = authorizationService;
    }

    public async Task<PaginatedDataDto> HandleAsync(GetMessages query)
    {
        var match = _dbContext.Matches.FirstOrDefault(x => x.Id.Equals(query.MatchId));
        if (match == null)
        {
            throw new MatchNotExistsException(query.MatchId);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(query.AuthenticatedUserId, match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        var dbQuery = _dbContext.Messages
                            .AsNoTracking()
                            .Where(x => x.MatchId.Equals(query.MatchId))
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