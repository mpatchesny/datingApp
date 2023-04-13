using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class SetMatchAsDisplayedHandler : ICommandHandler<SetMatchAsDisplayed>
{
    private readonly IMatchRepository _matchRepository;
    public SetMatchAsDisplayedHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public async Task HandleAsync(SetMatchAsDisplayed command)
    {
        var match = await _matchRepository.GetByIdAsync(command.MatchId);
        if (match == null)
        {
            throw new MatchNotExistsException(command.MatchId);
        }
        match.SetDisplayed(command.UserId);
        await _matchRepository.UpdateAsync(match);
    }
}