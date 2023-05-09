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
[Route("pass")]
public class PassController : ApiControllerBase
{
    private readonly ICommandHandler<SwipeUser> _swipeUserHandler;
    private readonly IQueryHandler<GetIsLikedByOtherUser, IsLikedByOtherUserDto> _getLikedByOtherUserHandler;
    public PassController(ICommandHandler<SwipeUser> swipeUserHandler,
                        IQueryHandler<GetIsLikedByOtherUser, IsLikedByOtherUserDto> getLikedByOtherUserHandler)
    {
        _swipeUserHandler = swipeUserHandler;
        _getLikedByOtherUserHandler = getLikedByOtherUserHandler;
    }

    [HttpPost("{userId:guid}")]
    public async Task<ActionResult<IsLikedByOtherUserDto>> Post(Guid userId)
    {
        var swipedById = Guid.Parse(User.Identity?.Name);
        var swipedWhoId = userId;
        var command = Authenticate(new SwipeUser(Guid.NewGuid(), swipedById, swipedWhoId, 1));
        await _swipeUserHandler.HandleAsync(command);

        var query = Authenticate(new GetIsLikedByOtherUser { SwipedById = swipedWhoId, SwipedWhoId = swipedById });
        var isLikedByOtherUser = await _getLikedByOtherUserHandler.HandleAsync(query);
        return isLikedByOtherUser;
    }
}