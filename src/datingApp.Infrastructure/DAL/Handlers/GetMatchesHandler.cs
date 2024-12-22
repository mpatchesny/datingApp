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

internal sealed class GetMatchesHandler : IQueryHandler<GetMatches, PaginatedDataDto<MatchDto>>
{
    private readonly ReadOnlyDatingAppDbContext _dbContext;
    private readonly ISpatial _spatial;
    public GetMatchesHandler(ReadOnlyDatingAppDbContext dbContext, ISpatial spatial)
    {
        _dbContext = dbContext;
        _spatial = spatial;
    }

    public async Task<PaginatedDataDto<MatchDto>> HandleAsync(GetMatches query)
    {
        if (!await _dbContext.Users.AnyAsync(user => user.Id.Equals(query.UserId)))
        {
            throw new UserNotExistsException(query.UserId);
        }

        var matches = await _dbContext.Matches
            .Include(match => match.Messages
                .OrderByDescending(message => message.CreatedAt)
                .Take(1))
            .Include(match => match.Users)
                .ThenInclude(user => user.Photos)
            .Include(match => match.Users)
                .ThenInclude(user => user.Settings)
            .Where(match => match.Users
                .Any(user => user.Id.Equals(query.UserId)))
            .OrderByDescending(match => match.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var recordsCount = await _dbContext.Matches
            .Where(match => match.Users
                .Any(user => user.Id.Equals(query.UserId)))
            .CountAsync();

        var pageCount = (recordsCount + query.PageSize - 1) / query.PageSize;

        var dataDto = new List<MatchDto>();
        foreach (var match in matches)
        {
            var distanceInKms = _spatial.CalculateDistanceInKms(match.Users.ElementAt(0), match.Users.ElementAt(1));
            dataDto.Add(match.AsDto(query.UserId, distanceInKms));
        }

        return dataDto.AsPaginatedDataDto(query.Page, query.PageSize, pageCount);
    }
}