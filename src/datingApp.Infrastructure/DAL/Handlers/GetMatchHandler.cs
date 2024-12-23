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
using datingApp.Core.Entities;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMatchHandler : IQueryHandler<GetMatch, MatchDto>
{
    private readonly ReadOnlyDatingAppDbContext _dbContext;
    private readonly ISpatial _spatial;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public GetMatchHandler(ReadOnlyDatingAppDbContext dbContext,
                        IDatingAppAuthorizationService authorizationService,
                        ISpatial spatial)
    {
        _dbContext = dbContext;
        _authorizationService = authorizationService;
        _spatial = spatial;
    }

    public async Task<MatchDto> HandleAsync(GetMatch query)
    {
        if (!await _dbContext.Users.AnyAsync(user => user.Id.Equals(query.UserId)))
        {
            throw new UserNotExistsException(query.UserId);
        }

        var match = await _dbContext.Matches
            .Include(match => match.Messages
                .OrderByDescending(message => message.CreatedAt)
                .Take(query.HowManyMessages))
            .Include(match => match.Users)
                .ThenInclude(user => user.Photos)
            .Include(match => match.Users)
                .ThenInclude(user => user.Settings)
            .Where(match => match.Id.Equals(query.MatchId))
            .FirstOrDefaultAsync();

        if (match == null) 
        {
            throw new MatchNotExistsException(query.MatchId);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(query.AuthenticatedUserId, match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        var distanceInKms = _spatial.CalculateDistanceInKms(match.Users.ElementAt(0), match.Users.ElementAt(1));

        return match.AsDto(query.UserId, distanceInKms);
    }
}