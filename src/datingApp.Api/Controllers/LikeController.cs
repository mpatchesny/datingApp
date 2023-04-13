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
public class LikeController : ControllerBase
{
    private readonly ICommandHandler<SwipeUser> _swipeUserHandler;
    private readonly IQueryHandler<GetMatch, IsMatchDto> _getMatchHandler;
    public LikeController(ICommandHandler<SwipeUser> swipeUserHandler,
                        IQueryHandler<GetMatch, IsMatchDto> getMatchHandler)
    {
        _swipeUserHandler = swipeUserHandler;
        _getMatchHandler = getMatchHandler;
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<IsMatchDto>> Get(Guid userId)
    {
        var swippedByUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var command = new SwipeUser(Guid.NewGuid(), swippedByUserId, userId, 2);
        await _swipeUserHandler.HandleAsync(command);
        var isMatch = await _getMatchHandler.HandleAsync(new GetMatch { SwipedById = command.SwipedById, SwipedWhoId = command.SwipedWhoId });
        return isMatch;
    }
}