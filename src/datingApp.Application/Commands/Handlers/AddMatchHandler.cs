using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class AddMatchHandler : ICommandHandler<AddMatch>
{
    private readonly IMatchRepository _matchRepository;
    private readonly ISwipeRepository _swipeRepository;
    public AddMatchHandler(IMatchRepository matchRepository,
                            ISwipeRepository swipeRepository)
    {
        _matchRepository = matchRepository;
        _swipeRepository = swipeRepository;
    }

    public async Task HandleAsync(AddMatch command)
    {
        Match match = new Match(Guid.NewGuid(), command.swippedById, command.swippedWhoId, false, false, null, DateTime.UtcNow);
        await _matchRepository.AddAsync(match);
    }
}