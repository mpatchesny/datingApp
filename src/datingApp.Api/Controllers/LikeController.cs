using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Application.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Api.Controllers;

[ApiController]
[Route("/")]
public class LikeController : ApiControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IIsLikedByOtherUserStorage _isLikedByOtherUserStorage;

    public LikeController(ICommandDispatcher commandDispatcher, IIsLikedByOtherUserStorage isLikedByOtherUserStorage)
    {
        _commandDispatcher = commandDispatcher;
        _isLikedByOtherUserStorage = isLikedByOtherUserStorage;
    }

    [HttpPut("like/{userId:guid}")]
    public async Task<ActionResult<IsLikedByOtherUserDto>> LikeUser(Guid userId)
    {
        var command = Authenticate(new SwipeUser(SwipedById: AuthenticatedUserId, SwipedWhoId: userId, 2));
        await _commandDispatcher.DispatchAsync(command);

        var isLikedByOtherUser = _isLikedByOtherUserStorage.Get();
        return isLikedByOtherUser;
    }

    [HttpPut("pass/{userId:guid}")]
    public async Task<ActionResult<IsLikedByOtherUserDto>> PassUser(Guid userId)
    {
        var command = Authenticate(new SwipeUser(SwipedById: AuthenticatedUserId, SwipedWhoId: userId, 1));
        await _commandDispatcher.DispatchAsync(command);

        var isLikedByOtherUser = _isLikedByOtherUserStorage.Get();
        return isLikedByOtherUser;
    }
}