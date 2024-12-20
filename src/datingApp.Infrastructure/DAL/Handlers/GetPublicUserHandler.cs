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
    private readonly ReadOnlyDatingAppDbContext _dbContext;
    private readonly ISpatial _spatial;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public GetPublicUserHandler(ReadOnlyDatingAppDbContext dbContext, ISpatial spatial, IDatingAppAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _spatial = spatial;
        _authorizationService = authorizationService;
    }

    public async Task<PublicUserDto> HandleAsync(GetPublicUser query)
    {
        var users = await _dbContext.Users
            .Include(user => user.Settings)
            .Include(user => user.Photos)
            .Include(user => user.Matches
                .Where(match => match.Users.Any(user => user.Id.Equals(query.RequestByUserId)))
                .Where(match => match.Users.Any(user => user.Id.Equals(query.RequestWhoUserId))))
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

        var match = requestedWho.Matches.FirstOrDefault();
        if (match == null)
        {
            throw new UnauthorizedException();
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(query.AuthenticatedUserId, match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        var distanceInKms = _spatial.CalculateDistanceInKms(requestedBy, requestedWho);
        return requestedWho.AsPublicDto(distanceInKms);
    }
}