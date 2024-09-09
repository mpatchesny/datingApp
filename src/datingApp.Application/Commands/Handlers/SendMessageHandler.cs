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
    private readonly IMessageRepository _messageRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IResourceAuthorizationService  _authorizationService;

    public SendMessageHandler(IMessageRepository messageRepository, IMatchRepository matchRepository, IResourceAuthorizationService authorizationService)
    {
        _messageRepository = messageRepository;
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

        var message = new Message(command.MessageId, command.MatchId, command.SendFromId, command.Text, false, DateTime.UtcNow);
        await _messageRepository.AddAsync(message);
    }
}