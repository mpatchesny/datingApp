using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Api.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IQueryHandler<GetPublicUser, PublicUserDto> _getUserHandler;

    public UserController(IQueryHandler<GetPublicUser, PublicUserDto> getUserHandler)
    {
        _getUserHandler = getUserHandler;
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<PublicUserDto>> Get(int userId)
    {
        var user = await _getUserHandler.HandleAsync(new GetPublicUser { UserId = userId });
        if (user is null)
        {
            return NotFound();
        }
        return user;
    }
}