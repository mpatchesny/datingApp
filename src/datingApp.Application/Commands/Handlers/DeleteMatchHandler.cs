using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public class DeleteMatchHandler : ICommandHandler<DeleteMatch>
{
    private readonly IMatchRepository _matchRepository;
    public DeleteMatchHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }
    public async Task HandleAsync(DeleteMatch command)
    {
        var match = await _matchRepository.GetByIdAsync(command.MatchId);
        if (match == null)
        {
            throw new MatchNotExistsException(command.MatchId);
        }
        await _matchRepository.DeleteAsync(match);
    }
}