using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Spatial;
using datingApp.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMatchesHandler : IQueryHandler<GetMatches, PaginatedDataDto>
{
    private readonly ReadOnlyDatingAppDbContext _dbContext;
    private readonly ISpatial _spatial;
    public GetMatchesHandler(ReadOnlyDatingAppDbContext dbContext, ISpatial spatial)
    {
        _dbContext = dbContext;
        _spatial = spatial;
    }

    public async Task<PaginatedDataDto> HandleAsync(GetMatches query)
    {
        if (!await _dbContext.Users.AnyAsync(user => user.Id.Equals(query.UserId)))
        {
            throw new UserNotExistsException(query.UserId);
        }

        var dbQuery = 
            from match in _dbContext.Matches
            .Include(match => match.Messages
                .OrderByDescending(message => message.CreatedAt)
                .Take(1))
            .Include(match => match.Users)
                .ThenInclude(user => user.Photos)
            .Include(match => match.Users)
                .ThenInclude(user => user.Settings)
            .Where(match => match.Users
                .Any(user => user.Id.Equals(query.UserId)))
            select match;

        var matches = await dbQuery
            .OrderByDescending(match => match.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        List<MatchDto> dataDto = new List<MatchDto>();
        foreach (var match in matches)
        {
            var user1 = match.Users.ElementAt(0);
            var user2 = match.Users.ElementAt(1);

            var distanceInKms = _spatial.CalculateDistanceInKms(user1, user2);

            var userDto = user1.Id.Equals(query.UserId) ? 
                user2.AsPublicDto(distanceInKms) :
                user1.AsPublicDto(distanceInKms);

            dataDto.Add(
                new MatchDto()
                {
                    Id = match.Id,
                    User = userDto,
                    IsDisplayed = match.IsDisplayedByUser(query.UserId),
                    Messages =  match.MessagesAsDto(),
                    CreatedAt = match.CreatedAt
                });
        }

        var pageCount = (await dbQuery.CountAsync() + query.PageSize - 1) / query.PageSize;

        return new PaginatedDataDto
        {
            Page = query.Page,
            PageSize = query.PageSize,
            PageCount = pageCount,
            Data = new List<dynamic>(dataDto)
        };
    }
}