using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Application.Commands.Handlers;

public sealed class SendMessageHandler : ICommandHandler<SendMessage>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IDatingAppAuthorizationService  _authorizationService;

    public SendMessageHandler(IMatchRepository matchRepository, IDatingAppAuthorizationService authorizationService)
    {
        _matchRepository = matchRepository;
        _authorizationService = authorizationService;
    }

    public async Task HandleAsync(SendMessage command)
    {
        Match match = await _matchRepository.GetByIdAsync(command.MatchId);
        if (match == null)
        {
            throw new MatchNotExistsException(command.MatchId);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(command.AuthenticatedUserId, match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        var message = new Message(command.MessageId, command.SendFromId, command.Text, false, DateTime.UtcNow);
        match.AddMessage(message);
        await _matchRepository.UpdateAsync(match);
    }
}