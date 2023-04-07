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
    private readonly ICommandHandler<SwipeUser> _swipeUserHandler;
    private readonly IQueryHandler<GetPrivateUser, PrivateUserDto> _getPrivateUserHandler;
    public SwipesController(IQueryHandler<GetSwipeCandidates, IEnumerable<PublicUserDto>> getSwipesCandidatesHandler,
                            ICommandHandler<SwipeUser> swipeUserHandler,
                            IQueryHandler<GetPrivateUser, PrivateUserDto> getPrivateUserHandler)
    {
        _getSwipesCandidatesHandler = getSwipesCandidatesHandler;
        _swipeUserHandler = swipeUserHandler;
        _getPrivateUserHandler = getPrivateUserHandler;
    }

    [HttpGet("candidates")]
    public async Task<ActionResult<IEnumerable<PublicUserDto>>> Get(int userId)
    {
        var user = await _getPrivateUserHandler.HandleAsync(new GetPrivateUser { UserId = userId });
        var command = new GetSwipeCandidates 
            { UserId = userId, 
            AgeFrom = user.Settings.DiscoverAgeFrom,
            AgeTo = user.Settings.DiscoverAgeTo,
            Range = user.Settings.DiscoverRange,
            HowMany = 10,
            Sex = user.Settings.DiscoverSex,
            Lat = user.Settings.Lat, 
            Lon = user.Settings.Lon };
        return Ok(await _getSwipesCandidatesHandler.HandleAsync(command));
    }

    [HttpPost]
    public async Task<ActionResult> Post(SwipeUser command)
    {
        await _swipeUserHandler.HandleAsync(command);
        return NoContent();
    }
}