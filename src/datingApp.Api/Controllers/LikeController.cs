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
[Route("like")]
public class LikeController : ControllerBase
{
    private readonly ICommandHandler<SwipeUser> _swipeUserHandler;
    private readonly IQueryHandler<GetIsLikedByOtherUser, IsLikedByOtherUserDto> _getLikedByOtherUserHandler;
    private readonly ICommandHandler<AddMatch> _addMatchHandler;
    public LikeController(ICommandHandler<SwipeUser> swipeUserHandler,
                        IQueryHandler<GetIsLikedByOtherUser, IsLikedByOtherUserDto> getLikedByOtherUserHandler,
                        ICommandHandler<AddMatch> addMatchHandler)
    {
        _swipeUserHandler = swipeUserHandler;
        _getLikedByOtherUserHandler = getLikedByOtherUserHandler;
        _addMatchHandler = addMatchHandler;
    }

    [HttpPost("{userId:guid}")]
    public async Task<ActionResult<IsLikedByOtherUserDto>> Get(Guid userId)
    {
        if (string.IsNullOrWhiteSpace(User.Identity?.Name)) return NotFound();
        var swipedById = Guid.Parse(User.Identity?.Name);
        var swipedWhoId = userId;
        var command = new SwipeUser(Guid.NewGuid(), swipedById, swipedWhoId, 2);
        await _swipeUserHandler.HandleAsync(command);

        var isLikedByOtherUser = await _getLikedByOtherUserHandler.HandleAsync(new GetIsLikedByOtherUser { SwipedById = swipedWhoId, SwipedWhoId = swipedById });
        if (isLikedByOtherUser.IsLikedByOtherUser) 
        {
            await _addMatchHandler.HandleAsync(new AddMatch(swipedById, swipedWhoId));
        }
        
        return isLikedByOtherUser;
    }
}