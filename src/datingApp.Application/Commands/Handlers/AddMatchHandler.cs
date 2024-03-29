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
    public AddMatchHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public async Task HandleAsync(AddMatch command)
    {
        Guid userId1 = command.swipedById;
        Guid userId2 = command.swipedWhoId;
        
        if (command.swipedById.CompareTo(command.swipedWhoId) >= 0)
        {
            userId1 = command.swipedWhoId;
            userId2 = command.swipedById;
        }

        var matchExists = await _matchRepository.ExistsAsync(userId1, userId2);
        if (matchExists) return;

        Match match = new Match(Guid.NewGuid(), userId1, userId2, false, false, null, DateTime.UtcNow);
        await _matchRepository.AddAsync(match);
    }
}