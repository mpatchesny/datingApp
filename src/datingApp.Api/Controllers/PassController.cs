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
[Route("pass")]
public class PassController : ControllerBase
{
    private readonly ICommandHandler<SwipeUser> _swipeUserHandler;
    public PassController(ICommandHandler<SwipeUser> swipeUserHandler)
    {
        _swipeUserHandler = swipeUserHandler;
    }

    [HttpPost("{userId:guid}")]
    public async Task<ActionResult<IsMatchDto>> Get(Guid userId)
    {
        if (string.IsNullOrWhiteSpace(User.Identity?.Name)) return NotFound();
        var swipedByUserId = Guid.Parse(User.Identity?.Name);
        var command = new SwipeUser(Guid.NewGuid(), swipedByUserId, userId, 1);
        await _swipeUserHandler.HandleAsync(command);

        var isLikedByOtherUser = new IsMatchDto {Match = false};
        return isLikedByOtherUser;
    }
}