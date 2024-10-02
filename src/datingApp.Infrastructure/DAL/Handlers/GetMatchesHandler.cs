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

internal sealed class GetMatchesHandler : IQueryHandler<GetMatches, PaginatedDataDto>
{
    private readonly DatingAppDbContext _dbContext;
    public GetMatchesHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedDataDto> HandleAsync(GetMatches query)
    {
        if (!await _dbContext.Users.AnyAsync(x => x.Id == query.UserId))
        {
            throw new UserNotExistsException(query.UserId);
        }

        var dbQuery = 
            from match in _dbContext.Matches.Include(m => m.Messages)
            from user in _dbContext.Users.Include(u => u.Photos)
            where user.Id == match.UserId1 || user.Id == match.UserId2
            where match.UserId1 == query.UserId || match.UserId2 == query.UserId
            where user.Id != query.UserId
            select new 
            {
                Match = match,
                User = user
            };

        var data = await dbQuery
                        .AsNoTracking()
                        .Skip((query.Page - 1) * query.PageSize)
                        .Take(query.PageSize)
                        .ToListAsync();

        List<MatchDto> dataDto = new List<MatchDto>();
        foreach (var x in data)
        {
            dataDto.Add(
                new MatchDto()
                {
                    Id = x.Match.Id,
                    User = x.User.AsPublicDto(0),
                    IsDisplayed = (x.Match.UserId1 == query.UserId) ? x.Match.IsDisplayedByUser1 : x.Match.IsDisplayedByUser2,
                    Messages =  x.Match.Messages.OrderByDescending(m => m.CreatedAt).Take(1).Select(x => x.AsDto()).ToList(),
                    CreatedAt = x.Match.CreatedAt
                });
        }

        var pageCount = (int) (dbQuery.Count() + query.PageSize - 1) / query.PageSize;

        return new PaginatedDataDto{
            Page = query.Page,
            PageSize = query.PageSize,
            PageCount = pageCount,
            Data = new List<dynamic>(dataDto)
            };
    }
}