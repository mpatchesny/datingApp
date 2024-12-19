using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class SetMessagesAsDisplayedHandler : ICommandHandler<SetMessagesAsDisplayed>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public SetMessagesAsDisplayedHandler(IMatchRepository matchRepository, IDatingAppAuthorizationService authorizationService)
    {
        _matchRepository = matchRepository;
        _authorizationService = authorizationService;
    }

    public async Task HandleAsync(SetMessagesAsDisplayed command)
    {
        var match = await _matchRepository.GetByMessageIdAsync(command.LastMessageId);
        if (match == null)
        {
            throw new MessageNotExistsException(command.LastMessageId);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(command.AuthenticatedUserId, match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        match.SetPreviousMessagesAsDisplayed(command.LastMessageId, command.DisplayedByUserId);
        await _matchRepository.UpdateAsync(match);
    }
}