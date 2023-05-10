using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
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
        var dbQuery = 
            from m in _dbContext.Matches
            from u in _dbContext.Users
            from p in _dbContext.Photos.Where(p => p.Oridinal == 0 && p.UserId == u.Id).DefaultIfEmpty()
            where u.Id == m.UserId1 || u.Id == m.UserId2
            where m.UserId1 == query.UserId || m.UserId2 == query.UserId 
            where u.Id != query.UserId
            select new 
            {
                Match = m,
                User = u,
                Photo = p,
            };

        var data = await dbQuery
                        .AsNoTracking()
                        .Skip((query.Page - 1) * query.PageSize)
                        .Take(query.PageSize)
                        .ToListAsync();

        var matchesId = data.Select(x => x.Match.Id).Distinct();

        var messagesList = await _dbContext.Messages
                        .AsNoTracking()
                        .Where(x => matchesId.Contains(x.MatchId))
                        .ToListAsync(); 

        List<MatchDto> dataDto = new List<MatchDto>();
        foreach (var x in data)
        {
            dataDto.Add(
                new MatchDto()
                {
                    Id = x.Match.Id,
                    User = await _dbContext.Users
                        .AsNoTracking()
                        .Where(u => u.Id == ((x.Match.UserId1 == query.UserId) ? x.Match.UserId2 : x.Match.UserId1))
                        .Select(u => u.AsPublicDto(0))
                        .FirstOrDefaultAsync(),
                    IsDisplayed = ((x.Match.UserId1 == query.UserId) ? x.Match.IsDisplayedByUser1 : x.Match.IsDisplayedByUser2),
                    Messages = messagesList.Where(m => m.MatchId == x.Match.Id).OrderByDescending(m => m.CreatedAt).Take(1).Select(x => x.AsDto()),
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