using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public class SetMessageAsDisplayedHandler : ICommandHandler<SetMessageAsDisplayed>
{
    private readonly IMessageRepository _messageRepository;
    public SetMessageAsDisplayedHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task HandleAsync(SetMessageAsDisplayed command)
    {
        var message = await _messageRepository.GetByIdAsync(command.MessageId);
        if (message == null)
        {
            throw new MessageNotExistsException(command.MessageId);
        }
        if (message.SendFromId == command.DisplayedByUserId)
        {
            throw new UserCannotSetMessageAsDisplayedException(command.MessageId);
        }
        
        message.SetDisplayed();
        await _messageRepository.UpdateAsync(message);
    }
}