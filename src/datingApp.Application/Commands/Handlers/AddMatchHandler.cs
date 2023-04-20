using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
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

    public Task HandleAsync(AddMatch command)
    {
        // TODO
        
        return Task.CompletedTask;
    }
}