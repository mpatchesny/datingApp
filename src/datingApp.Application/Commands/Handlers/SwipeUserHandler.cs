using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class SwipeUserHandler : ICommandHandler<SwipeUser>
{
    private readonly ISwipeRepository _swipeRepository;
    public SwipeUserHandler(ISwipeRepository swipeRepository)
    {
        _swipeRepository = swipeRepository;
    }

    public async Task HandleAsync(SwipeUser command)
    {
        var swipe = new Swipe(0, command.SwipedById, command.SwipedWhoId, (Like) command.Like, DateTime.UtcNow);
        await _swipeRepository.AddAsync(swipe);
    }
}