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
[Route("swipes")]
public class SwipesController : ControllerBase
{
    private readonly IQueryHandler<GetSwipeCandidates, IEnumerable<PublicUserDto>> _getSwipesCandidatesHandler;
    private readonly IQueryHandler<GetPrivateUser, PrivateUserDto> _getPrivateUserHandler;
    public SwipesController(IQueryHandler<GetSwipeCandidates, IEnumerable<PublicUserDto>> getSwipesCandidatesHandler,
                            ICommandHandler<SwipeUser> swipeUserHandler,
                            IQueryHandler<GetPrivateUser, PrivateUserDto> getPrivateUserHandler)
    {
        _getSwipesCandidatesHandler = getSwipesCandidatesHandler;
        _getPrivateUserHandler = getPrivateUserHandler;
    }

    [HttpGet("candidates")]
    public async Task<ActionResult<IEnumerable<PublicUserDto>>> Get()
    {
        Guid userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var user = await _getPrivateUserHandler.HandleAsync(new GetPrivateUser { UserId = userId });
        var command = new GetSwipeCandidates(user.Settings);
        return Ok(await _getSwipesCandidatesHandler.HandleAsync(command));
    }
}