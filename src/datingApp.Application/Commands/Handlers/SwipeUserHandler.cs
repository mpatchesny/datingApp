using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class SwipeUserHandler : ICommandHandler<SwipeUser>
{
    private readonly ISwipeRepository _swipeRepository;
    private readonly IUserRepository _userRepository;
    public SwipeUserHandler(ISwipeRepository swipeRepository, IUserRepository userRepository)
    {
        _swipeRepository = swipeRepository;
        _userRepository = userRepository;
    }

    public async Task HandleAsync(SwipeUser command)
    {
        var user = await _userRepository.GetByIdAsync(command.SwipedWhoId);
        if (user == null)
        {
            throw new UserNotExistsException(command.SwipedWhoId);
        }

        var swipe = new Swipe(command.SwipeId, command.SwipedById, command.SwipedWhoId, (Like) command.Like, DateTime.UtcNow);
        await _swipeRepository.AddAsync(swipe);
    }
}