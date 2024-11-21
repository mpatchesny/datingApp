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
    private readonly DatingAppDbContext _dbContext;
    private readonly ISpatial _spatial;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public GetMatchHandler(DatingAppDbContext dbContext, IDatingAppAuthorizationService authorizationService, ISpatial spatial)
    {
        _dbContext = dbContext;
        _authorizationService = authorizationService;
        _spatial = spatial;
    }

    public async Task<MatchDto> HandleAsync(GetMatch query)
    {
        var requestedBy = await _dbContext.Users
            .AsNoTracking()
            .Include(user => user.Settings)
            .FirstOrDefaultAsync(user => user.Id.Equals(query.UserId));

        if (requestedBy == null)
        {
            throw new UserNotExistsException(query.UserId);
        }

        var dbQuery =  
            from match in _dbContext.Matches
                .Include(match => match.Messages
                    .OrderByDescending(message => message.CreatedAt)
                    .Take(query.HowManyMessages))
            from user in _dbContext.Users
                .Include(user => user.Photos)
                .Include(user => user.Settings)
            where !user.Id.Equals(query.UserId)
            where match.Id.Equals(query.MatchId)
            where match.UserId1.Equals(user.Id) || match.UserId2.Equals(user.Id)
            select new 
            {
                Match = match,
                User = user
            };

        var data = await dbQuery
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (data == null) 
        {
            throw new MatchNotExistsException(query.MatchId);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(query.AuthenticatedUserId, data.Match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        var distanceInKms = _spatial.CalculateDistanceInKms(requestedBy.Settings.Location.Lat, requestedBy.Settings.Location.Lon, 
            data.User.Settings.Location.Lat, data.User.Settings.Location.Lon);

        return new MatchDto
        {
            Id = data.Match.Id,
            User = data.User.AsPublicDto(distanceInKms),
            IsDisplayed = data.Match.IsDisplayedByUser(query.UserId),
            Messages = data.Match.MessagesAsDto(),
            CreatedAt = data.Match.CreatedAt
        };
    }
}