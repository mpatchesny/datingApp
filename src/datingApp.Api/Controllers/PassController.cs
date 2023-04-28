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
    private readonly IQueryHandler<GetMatch, IsMatchDto> _getMatchHandler;
    public PassController(ICommandHandler<SwipeUser> swipeUserHandler,
                        IQueryHandler<GetMatch, IsMatchDto> getMatchHandler)
    {
        _swipeUserHandler = swipeUserHandler;
        _getMatchHandler = getMatchHandler;
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<IsMatchDto>> Get(Guid userId)
    {
        if (string.IsNullOrWhiteSpace(User.Identity?.Name)) return NotFound();
        var swippedByUserId = Guid.Parse(User.Identity?.Name);
        var command = new SwipeUser(Guid.NewGuid(), swippedByUserId, userId, 1);
        await _swipeUserHandler.HandleAsync(command);
        var isMatch = await _getMatchHandler.HandleAsync(new GetMatch { SwipedById = command.SwippedById, SwipedWhoId = command.SwippedWhoId });
        return isMatch;
    }
}