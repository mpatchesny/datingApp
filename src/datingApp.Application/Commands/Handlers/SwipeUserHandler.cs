using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class SwipeUserHandler : ICommandHandler<SwipeUser>
{
    private readonly ISwipeRepository _swipeRepository;
    private readonly IMatchRepository _matchRepository;
    public SwipeUserHandler(ISwipeRepository swipeRepository, IMatchRepository matchRepository)
    {
        _swipeRepository = swipeRepository;
        _matchRepository = matchRepository;
    }

    public async Task HandleAsync(SwipeUser command)
    {
        var swipeExists = await _swipeRepository.SwipeExists(command.SwipedById, command.SwipedWhoId);
        if (!swipeExists)
        {
            var swipe = new Swipe(command.SwipedById, command.SwipedWhoId, (Like) command.Like, DateTime.UtcNow);
            await _swipeRepository.AddAsync(swipe);

            if ((Like) command.Like == Like.Like)
            {
                var otherUserSwipe = await _swipeRepository.GetBySwipedBy(command.SwipedWhoId, command.SwipedById);
                if (otherUserSwipe?.Like == Like.Like)
                {
                    var match = new Match(Guid.NewGuid(), command.SwipedById, command.SwipedWhoId, false, false, null, DateTime.UtcNow);
                    await _matchRepository.AddAsync(match);
                    // return isLikedByOther user information via cache?
                }
            }
        }
    }
}