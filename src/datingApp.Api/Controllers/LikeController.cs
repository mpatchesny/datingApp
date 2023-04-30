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
    private readonly IQueryHandler<GetMatch, IsMatchDto> _getMatchHandler;
    private readonly ICommandHandler<AddMatch> _addMatchHandler;
    public LikeController(ICommandHandler<SwipeUser> swipeUserHandler,
                        IQueryHandler<GetMatch, IsMatchDto> getMatchHandler,
                        ICommandHandler<AddMatch> addMatchHandler)
    {
        _swipeUserHandler = swipeUserHandler;
        _getMatchHandler = getMatchHandler;
        _addMatchHandler = addMatchHandler;
    }

    [HttpPost("{userId:guid}")]
    public async Task<ActionResult<IsMatchDto>> Get(Guid userId)
    {
        if (string.IsNullOrWhiteSpace(User.Identity?.Name)) return NotFound();
        var swippedByUserId = Guid.Parse(User.Identity?.Name);
        var command = new SwipeUser(Guid.NewGuid(), swippedByUserId, userId, 2);
        await _swipeUserHandler.HandleAsync(command);
        await _addMatchHandler.HandleAsync(new AddMatch(swippedByUserId, userId));
        
        var isMatch = await _getMatchHandler.HandleAsync(new GetMatch { SwipedById = command.SwippedById, SwipedWhoId = command.SwippedWhoId });
        return isMatch;
    }
}