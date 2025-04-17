using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        var thisUser = await _dbContext.Users
            .Include(user => user.Photos)
            .Include(user => user.Settings)
            .FirstOrDefaultAsync(user => user.Id.Equals(query.UserId));

        if (thisUser == null)
        {
            throw new UserNotExistsException(query.UserId);
        }

        // UpdateDto:
        // event: new/ updated/ deleted user/match/message
        // object: user, match, message
        // changedTime

        var changedUsers = await _dbContext.Users
            .Include(user => user.Photos)
            .Include(user => user.Settings)
            .Include(user => user.Matches
                .Where(match => match.Users.Any(user => user.Id.Equals(query.UserId)))
            )
            .Where(user => user.UpdatedAt >= query.LastActivityTime)
            .Where(user => !user.Id.Equals(query.UserId))
            .Select(user => user.AsPublicDto(
                _spatial.CalculateDistanceInKms(thisUser, user)
            ))
            .ToListAsync();

        var newMessages = _dbContext.Matches
            .Include(match => match.Messages
                .Where(message => message.CreatedAt >= query.LastActivityTime))
                .OrderByDescending(message => message.CreatedAt)
            .Include(match => match.Users)
                .ThenInclude(user => user.Photos)
            .Include(match => match.Users)
                .ThenInclude(user => user.Settings)
            .Where(match => match.Messages.Any(message => message.CreatedAt >= query.LastActivityTime))
            .Where(match => match.Users.Any(user => user.Id.Equals(query.UserId)));

        var newMatches = _dbContext.Matches
            .Include(match => match.Messages
                .Where(message => message.CreatedAt >= query.LastActivityTime))
                .OrderByDescending(message => message.CreatedAt)
                .Take(1)
            .Include(match => match.Users)
                .ThenInclude(user => user.Photos)
            .Include(match => match.Users)
                .ThenInclude(user => user.Settings)
            .Where(match => match.LastActivityTime >= query.LastActivityTime)
            .Where(match => match.Users.Any(user => user.Id.Equals(query.UserId)));

        var newMatchesAndMessages2 = await newMessages.Union(newMatches)
            .Select(match => match.AsDto(
                query.UserId, 
                _spatial.CalculateDistanceInKms(match.Users.ElementAt(0), match.Users.ElementAt(1))
            ))
            .ToListAsync();

        var deletedMatches = new List<Match>();

        var newMatchesAndMessages = _dbContext.Matches
            .Include(match => match.Messages
                .Where(message => message.CreatedAt >= query.LastActivityTime))
                .OrderByDescending(message => message.CreatedAt)
            .Include(match => match.Users)
                .ThenInclude(user => user.Photos)
            .Include(match => match.Users)
                .ThenInclude(user => user.Settings)
            .Where(match => match.LastActivityTime >= query.LastActivityTime)
            .Where(match => match.Users.Any(user => user.Id.Equals(query.UserId)));

        var updates = newMatchesAndMessages;

        // FIXME: order by match.LastActivityTime if new match/message
        // or user.UpdatedAt if user changed
        var updatesMaterialized = await updates
            .OrderByDescending(match => match.LastActivityTime) 
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var updatesDto = updatesMaterialized
            .Select(match => match.AsDto(query.UserId, 
                _spatial.CalculateDistanceInKms(match.Users.First(), match.Users.Last())))
            .ToList();

        var recordsCount = await updates.CountAsync();

        var pageCount = (recordsCount + query.PageSize - 1) / query.PageSize;

        return updatesDto.AsPaginatedDataDto(query.Page, query.PageSize, pageCount);
    }
}