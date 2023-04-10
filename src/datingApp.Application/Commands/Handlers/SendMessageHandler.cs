using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class SendMessageHandler : ICommandHandler<SendMessage>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMatchRepository _matchRepository;
    public SendMessageHandler(IMessageRepository messageRepository, IMatchRepository matchRepository)
    {
        _messageRepository = messageRepository;
        _matchRepository = matchRepository;
    }

    public async Task HandleAsync(SendMessage command)
    {
        Match match = await _matchRepository.GetByIdAsync(command.MatchId);
        if (match == null)
        {
            throw new MatchNotExistsException(command.MatchId);
        }

        var message = new Message(command.MessageId, command.MatchId, command.SendToId, command.Text, false, DateTime.UtcNow);
        await _messageRepository.AddAsync(message);
    }
}