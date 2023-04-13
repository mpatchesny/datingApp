using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public class ChangeMessageHandler : ICommandHandler<ChangeMessage>
{
    private readonly IMessageRepository _messageRepository;
    public ChangeMessageHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task HandleAsync(ChangeMessage command)
    {
        var message = await _messageRepository.GetByIdAsync(command.MessageId);
        if (message == null)
        {
            throw new MessageNotExistsException(command.MessageId);
        }
        
        message.SetDisplayed();
        await _messageRepository.UpdateAsync(message);
    }
}