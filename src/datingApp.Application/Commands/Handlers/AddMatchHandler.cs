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
        var swipes = await _swipeRepository.GetByUserIdAsync(command.swippedById, command.swippedWhoId);
        if (swipes.Count() < 2) return;
        var passes = swipes.Where(x => x.Like == Like.Pass);
        if (passes.Count() > 0) return;

        Match match = new Match(Guid.NewGuid(), command.swippedById, command.swippedWhoId, false, false, null, DateTime.UtcNow);
        await _matchRepository.AddAsync(match);
    }
}