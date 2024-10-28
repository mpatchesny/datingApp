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
                            .Include(x => x.Settings)
                            .Include(x => x.Photos)
                            .Where(x => x.Id == query.RequestByUserId || x.Id == query.RequestWhoUserId)
                            .ToListAsync();

        var requestedWho = users.FirstOrDefault(x => x.Id == query.RequestWhoUserId);
        if (requestedWho == null) 
        {
            return null;
        };

        var requestedBy = users.FirstOrDefault(x => x.Id == query.RequestByUserId);
        if (requestedBy == null)
        {
            throw new UserNotExistsException(query.RequestByUserId);
        }

        // user who requested information about other user must be in pair (have match) with other user
        var match = await _dbContext.Matches
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => (m.UserId1 == requestedBy.Id || m.UserId2 == requestedBy.Id) && 
                            (m.UserId1 == requestedWho.Id || m.UserId2 == requestedWho.Id));

        var authorizationResult = await _authorizationService.AuthorizeAsync(query.AuthenticatedUserId, match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        var distanceInKms = _spatial.CalculateDistanceInKms(requestedBy.Settings.Location.Lat, requestedBy.Settings.Location.Lon, requestedWho.Settings.Location.Lat, requestedWho.Settings.Location.Lon);
        return requestedWho.AsPublicDto(distanceInKms);
    }
}