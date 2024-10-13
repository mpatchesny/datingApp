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
[Authorize]
[Route("like")]
public class LikeController : ApiControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IIsLikedByOtherUserStorage _isLikedByOtherUserStorage;

    public LikeController(ICommandDispatcher commandDispatcher, IIsLikedByOtherUserStorage isLikedByOtherUserStorage)
    {
        _commandDispatcher = commandDispatcher;
        _isLikedByOtherUserStorage = isLikedByOtherUserStorage;
    }

    [HttpPut("{userId:guid}")]
    public async Task<ActionResult<IsLikedByOtherUserDto>> Put(Guid userId)
    {
        var swipedById = AuthenticatedUserId;
        var swipedWhoId = userId;
        var command = Authenticate(new SwipeUser(swipedById, swipedWhoId, 2));
        await _commandDispatcher.DispatchAsync(command);

        var isLikedByOtherUser = _isLikedByOtherUserStorage.Get();
        return isLikedByOtherUser;
    }
}