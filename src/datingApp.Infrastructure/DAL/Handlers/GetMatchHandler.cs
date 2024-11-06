using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Core.Entities;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMatchHandler : IQueryHandler<GetMatch, MatchDto>
{
    private readonly DatingAppDbContext _dbContext;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public GetMatchHandler(DatingAppDbContext dbContext, IDatingAppAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _authorizationService = authorizationService;
    }

    public async Task<MatchDto> HandleAsync(GetMatch query)
    {
        var dbQuery = 
            from match in _dbContext.Matches
                .Include(match => match.Messages
                    .OrderByDescending(message => message.CreatedAt)
                    .Take(query.HowManyMessages))
            from user in _dbContext.Users.Include(u => u.Photos)
            where match.Id.Equals(query.MatchId)
            where match.UserId1.Equals(user.Id) || match.UserId2.Equals(user.Id)
            where !user.Id.Equals(query.UserId)
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

        return new MatchDto
        {
            Id = data.Match.Id,
            User = data.User.AsPublicDto(0),
            IsDisplayed = (data.Match.UserId1.Equals(query.UserId)) ? data.Match.IsDisplayedByUser1 : data.Match.IsDisplayedByUser2,
            Messages = data.Match.MessagesListAsDto(),
            CreatedAt = data.Match.CreatedAt
        }; 
    }
}