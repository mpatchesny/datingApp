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
using MailKit;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMessagesHandler : IQueryHandler<GetMessages, PaginatedDataDto>
{
    private readonly DatingAppDbContext _dbContext;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public GetMessagesHandler(DatingAppDbContext dbContext, IDatingAppAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _authorizationService = authorizationService;
    }

    public async Task<PaginatedDataDto> HandleAsync(GetMessages query)
    {
        var match = await GetMatchAsync(query.MatchId, query.Page, query.PageSize);

        if (match == null)
        {
            throw new MatchNotExistsException(query.MatchId);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(query.AuthenticatedUserId, match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        var messagesDto = MessagesToListOfMessagesDto(match);

        var recordsCount = await _dbContext.Matches
                        .Where(match => match.Id.Equals(query.MatchId))
                        .AsNoTracking()
                        .SelectMany(match => match.Messages)
                        .CountAsync();

        var pageCount = (int)(recordsCount + query.PageSize - 1) / query.PageSize;

        return new PaginatedDataDto
        {
            Page = query.Page,
            PageSize = query.PageSize,
            PageCount = pageCount,
            Data = new List<dynamic>(messagesDto)
        };
    }

    private async Task<Match> GetMatchAsync(Guid matchId, int page, int pageSize)
    {
        return await _dbContext.Matches
            .AsNoTracking()
            .Where(match => match.Id.Equals(matchId))
            .Include(match =>
                match.Messages.OrderByDescending(message => message.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize))
            .FirstOrDefaultAsync();
    }

    private static List<MessageDto> MessagesToListOfMessagesDto(Match match)
    {
        var messages = new List<MessageDto>();
        foreach (var message in match.Messages)
        {
            messages.Add(new MessageDto
            {
                Id = message.Id,
                MatchId = match.Id,
                SendFromId = message.SendFromId,
                Text = message.Text,
                IsDisplayed = message.IsDisplayed,
                CreatedAt = message.CreatedAt
            });
        }
        return messages;
    }
}