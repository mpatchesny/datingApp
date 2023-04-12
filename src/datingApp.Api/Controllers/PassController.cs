using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Api.Controllers;

[ApiController]
[Route("like")]
public class PassController : ControllerBase
{
    private readonly ICommandHandler<SwipeUser> _swipeUserHandler;
    private readonly IQueryHandler<GetMatch, IsMatchDto> _getMatchHandler;
    public PassController(ICommandHandler<SwipeUser> swipeUserHandler,
                        IQueryHandler<GetMatch, IsMatchDto> getMatchHandler)
    {
        _swipeUserHandler = swipeUserHandler;
        _getMatchHandler = getMatchHandler;
    }

    [HttpPost("{userId:guid}")]
    public async Task<ActionResult> Post(Guid userId)
    {
        var swippedUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var command = new SwipeUser(Guid.NewGuid(), swippedUserId, userId, 1);
        await _swipeUserHandler.HandleAsync(command);
        var something = await _getMatchHandler.HandleAsync(new GetMatch { SwipedById = command.SwipedById, SwipedWhoId = command.SwipedWhoId });
        return CreatedAtAction(nameof(Post), something);
    }
}