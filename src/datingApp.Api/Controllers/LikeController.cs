using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("like")]
public class LikeController : ApiControllerBase
{
    private readonly ICommandHandler<SwipeUser> _swipeUserHandler;
    public LikeController(ICommandHandler<SwipeUser> swipeUserHandler)
    {
        _swipeUserHandler = swipeUserHandler;
    }

    [HttpPut("{userId:guid}")]
    public async Task<ActionResult<IsLikedByOtherUserDto>> Put(Guid userId)
    {
        var swipedById = AuthenticatedUserId;
        var swipedWhoId = userId;
        var command = Authenticate(new SwipeUser(swipedById, swipedWhoId, 2));
        await _swipeUserHandler.HandleAsync(command);

        return isLikedByOtherUser;
    }
}