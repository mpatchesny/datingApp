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

internal sealed class GetMessagesHandler : IQueryHandler<GetMessages, PaginatedDataDto<MessageDto>>
{
    private readonly ReadOnlyDatingAppDbContext _dbContext;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public GetMessagesHandler(ReadOnlyDatingAppDbContext dbContext, IDatingAppAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _authorizationService = authorizationService;
    }

    public async Task<PaginatedDataDto<MessageDto>> HandleAsync(GetMessages query)
    {
        var match = await _dbContext.Matches
            .Where(match => match.Id.Equals(query.MatchId))
            .Include(match => match.Messages
                .OrderByDescending(message => message.CreatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize))
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

        var recordsCount = await _dbContext.Matches
            .Where(match => match.Id.Equals(query.MatchId))
            .SelectMany(match => match.Messages)
            .CountAsync();

        var pageCount = (recordsCount + query.PageSize - 1) / query.PageSize;

        return match.MessagesAsDto().AsPaginatedDataDto(query.Page, query.PageSize, pageCount);
    }
}