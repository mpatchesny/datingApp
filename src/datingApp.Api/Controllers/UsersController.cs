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
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IQueryHandler<GetPublicUser, PublicUserDto> _getPublicUserHandler;
    private readonly IQueryHandler<GetPrivateUser, PrivateUserDto> _getPrivateUserHandler;
    private readonly ICommandHandler<SignUp> _signUpHandler;
    private readonly ICommandHandler<ChangeUser> _changeUserHandler;
    private readonly ICommandHandler<DeleteUser> _deleteUserHandler;
    private readonly IQueryHandler<GetSwipeCandidates, IEnumerable<PublicUserDto>> _getSwipesCandidatesHandler;

    public UserController(IQueryHandler<GetPublicUser, PublicUserDto> getUserHandler,
                            ICommandHandler<SignUp> signUpHandler,
                            IQueryHandler<GetPrivateUser, PrivateUserDto> getPrivateUserHandler,
                            ICommandHandler<ChangeUser> changeUserHandler,
                            ICommandHandler<DeleteUser> deleteUserHandler,
                            IQueryHandler<GetSwipeCandidates, IEnumerable<PublicUserDto>> getSwipesCandidatesHandler)
    {
        _getPublicUserHandler = getUserHandler;
        _signUpHandler = signUpHandler;
        _getPrivateUserHandler = getPrivateUserHandler;
        _changeUserHandler = changeUserHandler;
        _deleteUserHandler = deleteUserHandler;
        _getSwipesCandidatesHandler = getSwipesCandidatesHandler;
    }

    [HttpGet("me")]
    public async Task<ActionResult<PrivateUserDto>> GetPrivateUser()
    {
        var user = await _getPrivateUserHandler.HandleAsync(new GetPrivateUser { UserId = Guid.Parse("00000000-0000-0000-0000-000000000001") });
        if (user is null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpGet("recommendations")]
    public async Task<ActionResult<IEnumerable<PublicUserDto>>> GetSwipeCandidates()
    {
        Guid userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var user = await _getPrivateUserHandler.HandleAsync(new GetPrivateUser { UserId = userId });
        var command = new GetSwipeCandidates(user.Settings);
        return Ok(await _getSwipesCandidatesHandler.HandleAsync(command));
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<PublicUserDto>> GetPublicUser(Guid userId)
    {
        var user = await _getPublicUserHandler.HandleAsync(new GetPublicUser { UserId = userId });
        if (user is null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post(SignUp command)
    {
        command = command with {UserId = Guid.NewGuid()};
        await _signUpHandler.HandleAsync(command);
        var user = await _getPrivateUserHandler.HandleAsync(new GetPrivateUser { UserId = command.UserId });
        return CreatedAtAction(nameof(GetPrivateUser), new { }, user);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Patch(ChangeUser command)
    {
        await _changeUserHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpDelete("{userId:guid}")]
    public async Task<ActionResult> Delete(Guid userId)
    {
        await _deleteUserHandler.HandleAsync(new DeleteUser(userId));
        return NoContent();
    }
}