using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Application.Spatial;
using datingApp.Infrastructure.Spatial;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetPublicUserHandler : IQueryHandler<GetPublicUser, PublicUserDto>
{
    private readonly DatingAppDbContext _dbContext;
    private readonly ISpatial _spatial;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public GetPublicUserHandler(DatingAppDbContext dbContext, ISpatial spatial, IDatingAppAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _spatial = spatial;
        _authorizationService = authorizationService;
    }

    public async Task<PublicUserDto> HandleAsync(GetPublicUser query)
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .Include(user => user.Settings)
            .Include(user => user.Photos)
            .Where(user => user.Id.Equals(query.RequestByUserId) || user.Id.Equals(query.RequestWhoUserId))
            .ToListAsync();

        var requestedWho = users.FirstOrDefault(user => user.Id.Equals(query.RequestWhoUserId));
        if (requestedWho == null) 
        {
            return null;
        };

        var requestedBy = users.FirstOrDefault(user => user.Id.Equals(query.RequestByUserId));
        if (requestedBy == null)
        {
            throw new UserNotExistsException(query.RequestByUserId);
        }

        // user who requested information about other user must be in pair (have match) with other user
        var match = await _dbContext.Matches
            .AsNoTracking()
            .Where(match => match.Users.Any(user => user.Id.Equals(requestedBy.Id)))
            .Where(match => match.Users.Any(user => user.Id.Equals(requestedWho.Id)))
            .FirstOrDefaultAsync();

        if (match == null)
        {
            throw new UnauthorizedException();
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(query.AuthenticatedUserId, match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        var distanceInKms = _spatial.CalculateDistanceInKms(requestedBy.Settings.Location.Lat, requestedBy.Settings.Location.Lon, 
            requestedWho.Settings.Location.Lat, requestedWho.Settings.Location.Lon);
        return requestedWho.AsPublicDto(distanceInKms);
    }
}