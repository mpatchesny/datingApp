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
    private readonly DatingAppDbContext _dbContext;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public GetMessageHandler(DatingAppDbContext dbContext, IDatingAppAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _authorizationService = authorizationService;
    }

    public async Task<MessageDto> HandleAsync(GetMessage query)
    {
        var dbQuery = 
            from match in _dbContext.Matches.Include(m => m.Messages)
            from message in _dbContext.Messages
            where match.Id == message.MatchId
            where message.Id == query.MessageId
            select new
            {
                Match = match,
                Message = message
            };

        var data = await dbQuery
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

        if (data.Message == null) 
        {
            throw new MessageNotExistsException(query.MessageId);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(query.AuthenticatedUserId, data.Match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        return data.Message.AsDto();
    }
}