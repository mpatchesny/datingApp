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
        var dbQuery = _dbContext.Matches
                        .AsNoTracking()
                        .Where(x => x.UserId1 == query.UserId || x.UserId2 == query.UserId);
        
        var data = await dbQuery.Select(x => new
                            {
                                Match = x,
                                User = _dbContext.Users.Where(u => u.Id == ((x.UserId1 != query.UserId) ? x.UserId1 : x.UserId2)).FirstOrDefault(),
                                Photo = _dbContext.Photos.Where(p => p.UserId == ((x.UserId1 != query.UserId) ? x.UserId1 : x.UserId2))
                                            .FirstOrDefault(p => p.Oridinal == 0)
                            })
                        .Select(x =>
                            new MatchDto()
                            {
                                Id = x.Match.Id,
                                UserId = x.User.Id,
                                Name = x.User.Name,
                                IsDisplayed = ((x.Match.UserId1 == query.UserId) ? x.Match.IsDisplayedByUser1 : x.Match.IsDisplayedByUser2),
                                ProfilePicture = x.Photo == null ? null : x.Photo.AsDto(),
                                Messages = x.Match.Messages.OrderByDescending(m => m.CreatedAt).Take(1).Select(x => x.AsDto()).ToList(),
                                CreatedAt = x.Match.CreatedAt
                            })
                        .Skip((query.Page - 1) * query.PageSize)
                        .Take(query.PageSize)
                        .ToListAsync();

        var pageCount = (int) (dbQuery.Count() + query.PageSize - 1) / query.PageSize;

        return new PaginatedDataDto{
            Page = query.Page,
            PageSize = query.PageSize,
            PageCount = pageCount,
            Data = new List<dynamic>(data)
            };
    }
}