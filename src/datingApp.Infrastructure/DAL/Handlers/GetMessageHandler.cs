using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMessageHandler : IQueryHandler<GetMessage, MessageDto>
{
    private readonly ReadOnlyDatingAppDbContext _dbContext;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public GetMessageHandler(ReadOnlyDatingAppDbContext dbContext, IDatingAppAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _authorizationService = authorizationService;
    }

    public async Task<MessageDto> HandleAsync(GetMessage query)
    {
        var match = await _dbContext.Matches
            .Include(match => match.Messages.Where(message => message.Id.Equals(query.MessageId)))
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (match == null || !match.Messages.Any())
        {
            throw new MessageNotExistsException(query.MessageId);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(query.AuthenticatedUserId, match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        return match.MessagesAsDto().FirstOrDefault();
    }
}