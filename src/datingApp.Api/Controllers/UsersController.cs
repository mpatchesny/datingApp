using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Core.ValueObjects;
using datingApp.Infrastructure.DAL.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("users")]
public class UserController : ApiControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ITokenStorage _tokenStorage;
    private readonly IAccessCodeStorage _codeStorage;
    private readonly IPhotoValidator<IFormFile> _photoService;

    public UserController(ICommandDispatcher commandDispatcher,
                          IQueryDispatcher queryDispatcher,
                          IAccessCodeStorage codeStorage,
                          ITokenStorage tokenStorage,
                          IPhotoValidator<IFormFile> photoService)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _codeStorage = codeStorage;
        _tokenStorage = tokenStorage;
        _photoService = photoService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<PrivateUserDto>> GetPrivateUser()
    {
        var query = Authenticate(new GetPrivateUser { UserId = AuthenticatedUserId });
        var user = await _queryDispatcher.DispatchAsync<GetPrivateUser, PrivateUserDto>(query);
        return user;
    }

    [HttpGet("me/recommendations")]
    public async Task<ActionResult<IEnumerable<PublicUserDto>>> GetSwipeCandidates()
    {
        var query = Authenticate(new GetSwipeCandidates { UserId = AuthenticatedUserId });
        var result = await _queryDispatcher.DispatchAsync<GetSwipeCandidates, IEnumerable<PublicUserDto>>(query);
        return Ok(result);
    }

    [HttpGet("me/updates")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetUpdates([FromQuery(Name = "lastActivityTime")] DateTime lastActivityTime)
    {
        var query = new GetUpdates();
        query.LastActivityTime = lastActivityTime;
        query = Authenticate(query);
        query.UserId = AuthenticatedUserId;
        var result = await _queryDispatcher.DispatchAsync<GetUpdates, IEnumerable<MatchDto>>(query);
        return Ok(result);
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<PublicUserDto>> GetPublicUser(Guid userId)
    {
        var query = Authenticate(new GetPublicUser { RequestByUserId = AuthenticatedUserId, RequestWhoUserId = userId });
        var user = await _queryDispatcher.DispatchAsync<GetPublicUser, PublicUserDto>(query);
        return user;
    }

    [HttpPatch("{userId:guid}")]
    public async Task<ActionResult> Patch([FromRoute] Guid userId, ChangeUser command)
    {
        command = Authenticate(command);
        command = command with {UserId = userId};
        await _commandDispatcher.DispatchAsync(command);
        return NoContent();
    }

    [HttpDelete("{userId:guid}")]
    public async Task<ActionResult> Delete(Guid userId)
    {
        var command = Authenticate(new DeleteUser(userId));
        await _commandDispatcher.DispatchAsync(command);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Post(SignUp command)
    {
        command = command with {UserId = Guid.NewGuid()};
        await _commandDispatcher.DispatchAsync(command);

        var query = new GetPrivateUser { UserId = command.UserId };
        var user = await _queryDispatcher.DispatchAsync<GetPrivateUser, PrivateUserDto>(query);
        return CreatedAtAction(nameof(GetPrivateUser), new {}, user);
    }

    [AllowAnonymous]
    [HttpPost("auth")]
    public async Task<ActionResult<string>> RequestAccessCode(RequestEmailAccessCode command)
    {
        await _commandDispatcher.DispatchAsync(command);
        var code = _codeStorage.Get(command.Email);
        var response = new { SendTo = command.Email, Code = code.AccessCode };
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("auth/refresh")]
    public async Task<ActionResult<JwtDto>> RefreshToken(RefreshJWT command)
    {
        await _commandDispatcher.DispatchAsync(command);
        var jwt = _tokenStorage.Get();
        return jwt;
    }

    [AllowAnonymous]
    [HttpPost("sign-in")]
    public async Task<ActionResult<JwtDto>> SingIn(SignInByEmail command)
    {
        await _commandDispatcher.DispatchAsync(command);
        var jwt = _tokenStorage.Get();
        return jwt;
    }

    [HttpPost("me/photos/")]
    public async Task<ActionResult> Post(IFormFile fileContent)
    {
        _photoService.Validate(fileContent, out var extension);

        var stream = new MemoryStream();
        await fileContent.CopyToAsync(stream);

        var command = Authenticate(new AddPhoto(Guid.NewGuid(), AuthenticatedUserId, stream));
        await _commandDispatcher.DispatchAsync(command);

        var query = Authenticate(new GetPhoto { PhotoId = command.PhotoId});
        var photo = await _queryDispatcher.DispatchAsync<GetPhoto, PhotoDto>(query);
        return CreatedAtAction(actionName: nameof(PhotosController.GetPhoto),
            controllerName: "Photos",
            routeValues: new { command.PhotoId },
            value: photo);
        // nameof(PhotosController.GetPhoto), new { command.PhotoId }, photo);
    }
}