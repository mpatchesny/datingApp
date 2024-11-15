using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMatchHandler : IQueryHandler<GetMatch, MatchDto>
{
    private readonly DatingAppDbContext _dbContext;
    private readonly DatingAppReadDbContext _readDbContext;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public GetMatchHandler(DatingAppDbContext dbContext, IDatingAppAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _authorizationService = authorizationService;
    }

    public async Task<MatchDto> HandleAsync(GetMatch query)
    {
        int limit = query.HowManyMessages;

        var altQuery = await _readDbContext.Matches
            .Where(match => match.Id == query.MatchId)
            .Include(match => match.Messages.OrderByDescending(message => message.CreatedAt).Take(limit))
            .Select(match => match.AsDto())
            .FirstOrDefaultAsync();

        var dbQuery = GetMatchesQuery(query.UserId, query.MatchId, limit: 1, offset: 0, messageOffset: 0, messageLimit: limit);
        var data = await dbQuery.FirstOrDefaultAsync();

        if (data.Match == null) 
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
            Messages = data.Match.MessagesAsDto(),
            CreatedAt = data.Match.CreatedAt
        }; 
    }

    private dynamic GetMatchesQuery(Guid userId, Guid matchId, int limit, int offset, int messageLimit, int messageOffset)
    {
        var query = from match in _dbContext.Matches
                .Include(match => match.Messages
                    .OrderByDescending(message => message.CreatedAt)
                    .Skip(messageOffset)
                    .Take(messageLimit)
                    .OrderBy(message => message.CreatedAt))
            from user in _dbContext.Users.Include(user => user.Photos)
            where !user.Id.Equals(userId)
            where match.Id.Equals(matchId)
            where match.UserId1.Equals(user.Id) || match.UserId2.Equals(user.Id)
            where match.UserId1.Equals(userId) || match.UserId2.Equals(userId)
            select new 
            {
                Match = match,
                User = user
            };

        return query
            .AsNoTracking()
            .OrderByDescending(item => item.Match.CreatedAt)
            .Skip(offset)
            .Take(limit);
    }
}