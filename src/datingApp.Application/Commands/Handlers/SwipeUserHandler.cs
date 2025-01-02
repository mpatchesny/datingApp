using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Storage;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class SwipeUserHandler : ICommandHandler<SwipeUser>
{
    private readonly ISwipeRepository _swipeRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;
    private readonly IIsLikedByOtherUserStorage _isLikedByOtherUserStorage;
    public SwipeUserHandler(ISwipeRepository swipeRepository,
                            IMatchRepository matchRepository,
                            IUserRepository userRepository,
                            IIsLikedByOtherUserStorage isLikedByOtherUserStorage)
    {
        _swipeRepository = swipeRepository;
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _isLikedByOtherUserStorage = isLikedByOtherUserStorage;
    }

    public async Task HandleAsync(SwipeUser command)
    {
        var user = await _userRepository.GetByIdAsync(command.SwipedWhoId);
        if (user == null)
        {
             throw new UserNotExistsException(command.SwipedWhoId);
        }

        var swipes = await _swipeRepository.GetBySwipedBy(command.SwipedById, command.SwipedWhoId);
        var swipe = swipes.FirstOrDefault(s => s.SwipedById.Equals(command.SwipedById));
        var otherUserSwipe = swipes.FirstOrDefault(s => s.SwipedById.Equals(command.SwipedWhoId));
        var repositoryCalls = new List<Task>();

        if (swipe == null)
        {
            swipe = new Swipe(command.SwipedById, command.SwipedWhoId, (Like) command.Like, DateTime.UtcNow);
            repositoryCalls.Add(_swipeRepository.AddAsync(swipe));

            if ((Like) command.Like == Like.Like)
            {
                user.AddLike();
                repositoryCalls.Add(_userRepository.UpdateAsync(user));
            }
        }

        if (otherUserSwipe?.Like == Like.Like && swipe.Like == Like.Like)
        {
            var users = new List<Guid>() { command.SwipedById, command.SwipedWhoId };
            users.Sort();
            var match = new Match(Guid.NewGuid(), users[0], users[1], DateTime.UtcNow);
            repositoryCalls.Add(_matchRepository.AddAsync(match));
        }

        foreach (var task in repositoryCalls)
        {
            task.RunSynchronously();
        }

        var isLikedByOtherUser = new IsLikedByOtherUserDto() { IsLikedByOtherUser = otherUserSwipe?.Like == Like.Like };
        _isLikedByOtherUserStorage.Set(isLikedByOtherUser);
    }
}