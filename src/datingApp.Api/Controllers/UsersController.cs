using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Infrastructure.DAL.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Api.Controllers;

[ApiController]
[Route("users")]
public class UserController : ApiControllerBase
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
    private readonly IQueryHandler<GetUpdates, IEnumerable<MatchDto>> _getUpdatesHandler;

    public UserController(IQueryHandler<GetPublicUser, PublicUserDto> getUserHandler,
                            ICommandHandler<SignUp> signUpHandler,
                            IQueryHandler<GetPrivateUser, PrivateUserDto> getPrivateUserHandler,
                            ICommandHandler<ChangeUser> changeUserHandler,
                            ICommandHandler<DeleteUser> deleteUserHandler,
                            IQueryHandler<GetSwipeCandidates, IEnumerable<PublicUserDto>> getSwipesCandidatesHandler,
                            ICommandHandler<RequestEmailAccessCode> requestAccessCodeHandler,
                            ICommandHandler<SignInByEmail> signInHandler,
                            ITokenStorage tokenStorage,
                            IAccessCodeStorage codeStorage,
                            IQueryHandler<GetUpdates, IEnumerable<MatchDto>> getUpdatesHandler)
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
        _getUpdatesHandler = getUpdatesHandler;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<PrivateUserDto>> GetPrivateUser()
    {
        var query = Authenticate(new GetPrivateUser { UserId = AuthenticatedUserId });
        var user = await _getPrivateUserHandler.HandleAsync(query);
        return user;
    }

    [Authorize]
    [HttpGet("me/recommendations")]
    public async Task<ActionResult<IEnumerable<PublicUserDto>>> GetSwipeCandidates()
    {
        // FIXME: magic number
        var command = Authenticate(new GetSwipeCandidates { UserId = AuthenticatedUserId, HowMany = 10 });
        return Ok(await _getSwipesCandidatesHandler.HandleAsync(command));
    }

    [Authorize]
    [HttpGet("me/updates")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetUpdates([FromQuery(Name = "lastActivityTime")] DateTime lastActivityTime)
    {
        var query = new GetUpdates();
        query.LastActivityTime = lastActivityTime;
        query = Authenticate(query);
        query.UserId = AuthenticatedUserId;
        return Ok(await _getUpdatesHandler.HandleAsync(query));
    }

    [Authorize]
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<PublicUserDto>> GetPublicUser(Guid userId)
    {
        var query = Authenticate(new GetPublicUser { UserId = userId, UserRequestedId = AuthenticatedUserId });
        var user = await _getPublicUserHandler.HandleAsync(query);
        return user;
    }

    [Authorize]
    [HttpPatch("{userId:guid}")]
    public async Task<ActionResult> Patch([FromRoute] Guid userId, ChangeUser command)
    {
        command = Authenticate(command);
        command = command with {UserId = userId};
        await _changeUserHandler.HandleAsync(command);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{userId:guid}")]
    public async Task<ActionResult> Delete(Guid userId)
    {
        var command = Authenticate(new DeleteUser(userId));
        await _deleteUserHandler.HandleAsync(command);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Post(SignUp command)
    {
        command = command with {UserId = Guid.NewGuid()};
        await _signUpHandler.HandleAsync(command);

        var query = new GetPrivateUser { UserId = command.UserId };
        var user = await _getPrivateUserHandler.HandleAsync(query);
        return CreatedAtAction(nameof(GetPrivateUser), new {}, user);
    }

    [AllowAnonymous]
    [HttpPost("auth")]
    public async Task<ActionResult<string>> RequestAccessCode(RequestEmailAccessCode command)
    {
        await _requestAccessCodeHandler.HandleAsync(command);
        var code = _codeStorage.Get(command.Email);
        var response = new { SendTo = command.Email, Code = code.AccessCode };
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("sign-in")]
    public async Task<ActionResult<JwtDto>> SingIn(SignInByEmail command)
    {
        await _signInHandler.HandleAsync(command);
        var jwt = _tokenStorage.Get();
        return jwt;
    }
}