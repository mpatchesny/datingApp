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

internal sealed class GetUpdatesHandler : IQueryHandler<GetUpdates, IEnumerable<MatchDto>>
{
    private readonly ReadOnlyDatingAppDbContext _dbContext;
    private readonly ISpatial _spatial;

    public GetUpdatesHandler(ReadOnlyDatingAppDbContext dbContext, ISpatial spatial)
    {
        _dbContext = dbContext;
        _spatial = spatial;
    }

    public async Task<IEnumerable<MatchDto>> HandleAsync(GetUpdates query)
    {
        if (!await _dbContext.Users.AnyAsync(user => user.Id.Equals(query.UserId)))
        {
            throw new UserNotExistsException(query.UserId);
        }

        var matches = await _dbContext.Matches
            .Include(match => match.Messages
                .Where(message => message.CreatedAt >= query.LastActivityTime))
            .Where(match => 
                match.Messages.Any(message => message.CreatedAt >= query.LastActivityTime) ||
                    match.CreatedAt >= query.LastActivityTime)
            .Include(match => match.Users)
                .ThenInclude(user => user.Photos)
            .Include(match => match.Users)
                .ThenInclude(user => user.Settings)
            .Where(match => match.Users
                .Any(user => user.Id.Equals(query.UserId)))
            .Select(match => match.AsDto(query.UserId, 
                _spatial.CalculateDistanceInKms(match.Users.ElementAt(0), match.Users.ElementAt(1))))
            .ToListAsync();

        return matches;
    }
}