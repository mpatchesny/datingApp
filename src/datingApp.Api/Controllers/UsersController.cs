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

    public UserController(IQueryHandler<GetPublicUser, PublicUserDto> getUserHandler,
                            ICommandHandler<SignUp> signUpHandler,
                            IQueryHandler<GetPrivateUser, PrivateUserDto> getPrivateUserHandler)
    {
        _getPublicUserHandler = getUserHandler;
        _signUpHandler = signUpHandler;
        _getPrivateUserHandler = getPrivateUserHandler;
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<PublicUserDto>> Get(int userId)
    {
        var user = await _getPublicUserHandler.HandleAsync(new GetPublicUser { UserId = userId });
        if (user is null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpPost]
    public async Task<ActionResult> Post(SignUp command)
    {
        await _signUpHandler.HandleAsync(command);
        // var user = await _getPublicUserHandler.HandleAsync(new GetPrivateUser { UserId });
        return CreatedAtAction(nameof(Get), null, null);
    }
}