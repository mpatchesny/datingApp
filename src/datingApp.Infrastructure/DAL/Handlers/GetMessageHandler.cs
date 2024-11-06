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
            where match.Messages.Any(m => m.Id.Equals(query.MessageId))
            select new
            {
                Match = match,
                Message = match.Messages.FirstOrDefault(m => m.Id.Equals(query.MessageId))
            };

        var data = await dbQuery
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

        if (data == null)
        {
            throw new MessageNotExistsException(query.MessageId);
        }
        else if (data.Message == null)
        {
            throw new MessageNotExistsException(query.MessageId);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(query.AuthenticatedUserId, data.Match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        return new MessageDto()
        {
            Id = data.Message.Id,
            MatchId = data.Match.Id, 
            SendFromId = data.Message.SendFromId,
            Text = data.Message.Text,
            IsDisplayed = data.Message.IsDisplayed,
            CreatedAt = data.Message.CreatedAt
        };
    }
}