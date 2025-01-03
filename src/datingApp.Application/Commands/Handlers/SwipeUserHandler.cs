using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Services;
using datingApp.Application.Storage;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class SwipeUserHandler : ICommandHandler<SwipeUser>
{
    private readonly ISwipeRepository _swipeRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IDeletedEntityService _deletedEntityService;
    private readonly IIsLikedByOtherUserStorage _isLikedByOtherUserStorage;
    public SwipeUserHandler(ISwipeRepository swipeRepository,
                            IMatchRepository matchRepository,
                            IDeletedEntityService deletedEntityService,
                            IIsLikedByOtherUserStorage isLikedByOtherUserStorage)
    {
        _swipeRepository = swipeRepository;
        _matchRepository = matchRepository;
        _deletedEntityService = deletedEntityService;
        _isLikedByOtherUserStorage = isLikedByOtherUserStorage;
    }

    public async Task HandleAsync(SwipeUser command)
    {
        var swipes = await _swipeRepository.GetBySwipedBy(command.SwipedById, command.SwipedWhoId);
        var swipe = swipes.FirstOrDefault(s => s.SwipedById.Equals(command.SwipedById));
        var otherUserSwipe = swipes.FirstOrDefault(s => s.SwipedById.Equals(command.SwipedWhoId));
        var isLikedByOtherUser = false;

        if (swipe == null)
        {
            swipe = new Swipe(command.SwipedById, command.SwipedWhoId, (Like) command.Like, DateTime.UtcNow);
            await _swipeRepository.AddAsync(swipe);
        }

        if (otherUserSwipe?.Like == Like.Like && swipe.Like == Like.Like)
        {
            if (!await _deletedEntityService.ExistsAsync(command.SwipedWhoId))
            {
                var match = new Match(Guid.NewGuid(), command.SwipedById, command.SwipedWhoId, DateTime.UtcNow);
                await _matchRepository.AddAsync(match);
                isLikedByOtherUser = true;
            }
        }

        var result = new IsLikedByOtherUserDto() { IsLikedByOtherUser = isLikedByOtherUser };
        _isLikedByOtherUserStorage.Set(result);
    }
}