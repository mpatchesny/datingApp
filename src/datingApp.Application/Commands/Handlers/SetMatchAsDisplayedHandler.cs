using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class SetMatchAsDisplayedHandler : ICommandHandler<SetMatchAsDisplayed>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public SetMatchAsDisplayedHandler(IMatchRepository matchRepository, IDatingAppAuthorizationService authorizationService)
    {
        _matchRepository = matchRepository;
        _authorizationService = authorizationService;
    }

    public async Task HandleAsync(SetMatchAsDisplayed command)
    {
        var match = await _matchRepository.GetByIdAsync(command.MatchId);
        if (match == null)
        {
            throw new MatchNotExistsException(command.MatchId);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(command.AuthenticatedUserId, match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        match.SetDisplayed(command.DisplayedByUserId);
        await _matchRepository.UpdateAsync(match);
    }
}