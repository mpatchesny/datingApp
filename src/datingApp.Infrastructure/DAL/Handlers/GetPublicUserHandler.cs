using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Security;
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
        var userRequested = await _dbContext.Users
                                        .AsNoTracking()
                                        .Where(x => x.Id == query.UserRequestedId)
                                        .Include(u => u.Settings)
                                        .FirstOrDefaultAsync();

        if (userRequested == null) return null;

        var userWhoRequested = await _dbContext.Users
                            .AsNoTracking()
                            .Include(x => x.Settings)
                            .Include(x => x.Photos)
                            .FirstOrDefaultAsync(x => x.Id == query.UserId);

        if (userWhoRequested == null)
        {
            throw new UserNotExistsException(query.UserId);
        }

        // user who requested information about other user must have match with other user
        var match = await _dbContext.Matches
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => (m.UserId1 == userRequested.Id || m.UserId2 == userRequested.Id) && 
                            (m.UserId1 == userWhoRequested.Id || m.UserId2 == userWhoRequested.Id));

        var authorizationResult = await _authorizationService.AuthorizeAsync(query.AuthenticatedUserId, match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        var distanceInKms = _spatial.CalculateDistanceInKms(userRequested.Settings.Lat, userRequested.Settings.Lon, userWhoRequested.Settings.Lat, userWhoRequested.Settings.Lon);
        return userWhoRequested.AsPublicDto(distanceInKms);
    }
}