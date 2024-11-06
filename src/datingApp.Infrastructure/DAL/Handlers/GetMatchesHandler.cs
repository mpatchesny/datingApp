using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
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
        if (!await _dbContext.Users.AnyAsync(x => x.Id.Equals(query.UserId)))
        {
            throw new UserNotExistsException(query.UserId);
        }

        var dbQuery = 
            from match in _dbContext.Matches
                .Include(match => match.Messages
                    .OrderByDescending(message => message.CreatedAt)
                    .Take(1))
            from user in _dbContext.Users.Include(user => user.Photos)
            where !user.Id.Equals(query.UserId)
            where match.UserId1.Equals(user.Id) || match.UserId2.Equals(user.Id)
            where match.UserId1.Equals(query.UserId) || match.UserId2.Equals(query.UserId)
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
        foreach (var item in data)
        {
            dataDto.Add(
                new MatchDto()
                {
                    Id = item.Match.Id,
                    User = item.User.AsPublicDto(0),
                    IsDisplayed = (item.Match.UserId1.Equals(query.UserId)) ? item.Match.IsDisplayedByUser1 : item.Match.IsDisplayedByUser2,
                    Messages =  item.Match.MessagesAsDto(),
                    CreatedAt = item.Match.CreatedAt
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