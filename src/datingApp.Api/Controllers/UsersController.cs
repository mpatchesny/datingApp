using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using Microsoft.AspNetCore.Authorization;
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
    private readonly ICommandHandler<RequestEmailAccessCode> _requestAccessCodeHandler;
    private readonly ICommandHandler<SignInByEmail> _signInHandler;
    private readonly ITokenStorage _tokenStorage;
    private readonly IAccessCodeStorage _codeStorage;

    public UserController(IQueryHandler<GetPublicUser, PublicUserDto> getUserHandler,
                            ICommandHandler<SignUp> signUpHandler,
                            IQueryHandler<GetPrivateUser, PrivateUserDto> getPrivateUserHandler,
                            ICommandHandler<ChangeUser> changeUserHandler,
                            ICommandHandler<DeleteUser> deleteUserHandler,
                            IQueryHandler<GetSwipeCandidates, IEnumerable<PublicUserDto>> getSwipesCandidatesHandler,
                            ICommandHandler<RequestEmailAccessCode> requestAccessCodeHandler,
                            ICommandHandler<SignInByEmail> signInHandler,
                            ITokenStorage tokenStorage,
                            IAccessCodeStorage codeStorage)
    {
        _getPublicUserHandler = getUserHandler;
        _signUpHandler = signUpHandler;
        _getPrivateUserHandler = getPrivateUserHandler;
        _changeUserHandler = changeUserHandler;
        _deleteUserHandler = deleteUserHandler;
        _getSwipesCandidatesHandler = getSwipesCandidatesHandler;
        _requestAccessCodeHandler = requestAccessCodeHandler;
        _signInHandler = signInHandler;
        _tokenStorage = tokenStorage;
        _codeStorage = codeStorage;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<PrivateUserDto>> GetPrivateUser()
    {
        if (string.IsNullOrWhiteSpace(User.Identity?.Name)) return NotFound();
        var userId = Guid.Parse(User.Identity?.Name);
        var user = await _getPrivateUserHandler.HandleAsync(new GetPrivateUser { UserId = userId });
        return user;
    }

    [Authorize]
    [HttpGet("recommendations")]
    public async Task<ActionResult<IEnumerable<PublicUserDto>>> GetSwipeCandidates()
    {
        if (string.IsNullOrWhiteSpace(User.Identity?.Name)) return NotFound();
        var userId = Guid.Parse(User.Identity?.Name);
        var user = await _getPrivateUserHandler.HandleAsync(new GetPrivateUser { UserId = userId });
        var command = new GetSwipeCandidates(user.Settings);
        return Ok(await _getSwipesCandidatesHandler.HandleAsync(command));
    }

    [Authorize]
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
    public async Task<ActionResult> Post(SignUp command)
    {
        command = command with {UserId = Guid.NewGuid()};
        await _signUpHandler.HandleAsync(command);
        var user = await _getPrivateUserHandler.HandleAsync(new GetPrivateUser { UserId = command.UserId });
        return CreatedAtAction(nameof(GetPrivateUser), new {}, user);
    }

    [Authorize]
    [HttpPatch]
    public async Task<ActionResult> Patch(ChangeUser command)
    {
        if (string.IsNullOrWhiteSpace(User.Identity?.Name)) return NotFound();
        var userId = Guid.Parse(User.Identity?.Name);
        command = command with {UserId = userId};
        await _changeUserHandler.HandleAsync(command);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{userId:guid}")]
    public async Task<ActionResult> Delete(Guid userId)
    {
        await _deleteUserHandler.HandleAsync(new DeleteUser(userId));
        return NoContent();
    }

    [HttpPost("auth")]
    public async Task<ActionResult<string>> RequestAccessCode(RequestEmailAccessCode command)
    {
        await _requestAccessCodeHandler.HandleAsync(command);
        var code = _codeStorage.Get(command.Email);
        var response = new { SendTo = command.Email, Code = code.AccessCode };
        return Ok(response);
    }

    [HttpPost("sing-in")]
    public async Task<ActionResult<JwtDto>> SingIn(SignInByEmail command)
    {
        await _signInHandler.HandleAsync(command);
        var jwt = _tokenStorage.Get();
        return jwt;
    }
}