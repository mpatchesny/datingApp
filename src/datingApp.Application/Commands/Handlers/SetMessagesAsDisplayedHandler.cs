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
    private readonly IMatchRepository _matchRepository;
    public SetMessagesAsDisplayedHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public async Task HandleAsync(SetMessagesAsDisplayed command)
    {
        var match = await _matchRepository.GetByMessageIdAsync(command.LastMessageId);
        match.SetPreviousMessagesAsDisplayed(command.LastMessageId, command.DisplayedByUserId);
        await _matchRepository.UpdateAsync(match);
    }
}