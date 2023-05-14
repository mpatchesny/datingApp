using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public class SetMessagesAsDisplayedHandler : ICommandHandler<SetMessagesAsDisplayed>
{
    private readonly IMessageRepository _messageRepository;
    public SetMessagesAsDisplayedHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task HandleAsync(SetMessagesAsDisplayed command)
    {
        var messages = (await _messageRepository.GetPreviousNotDisplayedMessages(command.LastMessageId))
                        .Where(x => x.SendFromId != command.DisplayedByUserId)
                        .ToList();

        if (messages.Count() == 0) return;
        
        foreach (var message in messages)
        {
            message.SetDisplayed();
        }
        await _messageRepository.UpdateRangeAsync(messages.ToArray());
    }
}