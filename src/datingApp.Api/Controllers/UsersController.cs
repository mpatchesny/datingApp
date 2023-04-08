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

    public UserController(IQueryHandler<GetPublicUser, PublicUserDto> getUserHandler,
                            ICommandHandler<SignUp> signUpHandler,
                            IQueryHandler<GetPrivateUser, PrivateUserDto> getPrivateUserHandler,
                            ICommandHandler<ChangeUser> changeUserHandler,
                            ICommandHandler<DeleteUser> deleteUserHandler)
    {
        _getPublicUserHandler = getUserHandler;
        _signUpHandler = signUpHandler;
        _getPrivateUserHandler = getPrivateUserHandler;
        _changeUserHandler = changeUserHandler;
        _deleteUserHandler = deleteUserHandler;
    }

    [HttpGet("me")]
    public async Task<ActionResult<PrivateUserDto>> GetPrivateUser()
    {
        var user = await _getPrivateUserHandler.HandleAsync(new GetPrivateUser { UserId = 1 });
        if (user is null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<PublicUserDto>> GetPublicUser(int userId)
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
        await _signUpHandler.HandleAsync(command);
        // var user = await _getPublicUserHandler.HandleAsync(new GetPrivateUser { UserId });
        return CreatedAtAction(nameof(GetPrivateUser), new { userId = 1 });
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Patch(ChangeUser command)
    {
        await _changeUserHandler.HandleAsync(command);
        // var user = await _getPublicUserHandler.HandleAsync(new GetPrivateUser { UserId });
        return CreatedAtAction(nameof(GetPrivateUser), new { userId = 1 });
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(int userId)
    {
        await _deleteUserHandler.HandleAsync(new DeleteUser(userId));
        return Ok();
    }
}