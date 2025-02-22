using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Spatial;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetUpdatesHandler : IQueryHandler<GetUpdates, PaginatedDataDto<MatchDto>>
{
    private readonly ReadOnlyDatingAppDbContext _dbContext;
    private readonly ISpatial _spatial;

    public GetUpdatesHandler(ReadOnlyDatingAppDbContext dbContext, ISpatial spatial)
    {
        _dbContext = dbContext;
        _spatial = spatial;
    }

    public async Task<PaginatedDataDto<MatchDto>> HandleAsync(GetUpdates query)
    {
        if (!await _dbContext.Users.AnyAsync(user => user.Id.Equals(query.UserId)))
        {
            throw new UserNotExistsException(query.UserId);
        }

        var matches = await _dbContext.Matches
            .Include(match => match.Messages
                .Where(message => message.CreatedAt >= query.LastActivityTime))
                .OrderByDescending(message => message.CreatedAt)
            .Include(match => match.Users)
                .ThenInclude(user => user.Photos)
            .Include(match => match.Users)
                .ThenInclude(user => user.Settings)
            .Where(match => match.LastActivityTime >= query.LastActivityTime)
            .Where(match => match.Users.Any(user => user.Id.Equals(query.UserId)))
            .OrderByDescending(match => match.LastActivityTime)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var matchesDto = matches
            .Select(match => match.AsDto(query.UserId, 
                _spatial.CalculateDistanceInKms(match.Users.First(), match.Users.Last())))
            .ToList();

        var recordsCount = await _dbContext.Matches
            .Where(match => match.LastActivityTime >= query.LastActivityTime)
            .Where(match => match.Users
                .Any(user => user.Id.Equals(query.UserId)))
            .CountAsync();

        var pageCount = (recordsCount + query.PageSize - 1) / query.PageSize;

        return matchesDto.AsPaginatedDataDto(query.Page, query.PageSize, pageCount);
    }
}