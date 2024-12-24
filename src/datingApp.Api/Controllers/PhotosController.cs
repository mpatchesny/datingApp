using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("photos")]
public class PhotosController : ApiControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;
    public PhotosController(ICommandDispatcher commandDispatcher,
                            IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("{photoId:guid}")]
    public async Task<ActionResult<PhotoDto>> GetPhoto(Guid photoId)
    {
        var query = Authenticate(new GetPhoto {PhotoId = photoId});
        var photo = await _queryDispatcher.DispatchAsync<GetPhoto, PhotoDto>(query);
        return photo;
    }

    [HttpPatch("{photoId:guid}")]
    public async Task<ActionResult> Patch([FromRoute] Guid photoId, ChangePhotoOridinal command)
    {
        command = Authenticate(command with {PhotoId = photoId});
        await _commandDispatcher.DispatchAsync(command);
        return NoContent();
    }

    [HttpDelete("{photoId:guid}")]
    public async Task<ActionResult> Delete(Guid photoId)
    {
        var command = Authenticate(new DeletePhoto(photoId));
        await _commandDispatcher.DispatchAsync(command);
        return NoContent();
    }
}