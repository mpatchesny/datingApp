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
    private readonly IQueryHandler<GetPhoto, PhotoDto> _getPhotoHandler;
    private readonly ICommandHandler<AddPhoto> _addPhotoHandler;
    private readonly ICommandHandler<ChangePhotoOridinal> _changePhotoOridinalHandler;
    private readonly ICommandHandler<DeletePhoto> _deletePhotoHandler;
    public PhotosController(ICommandHandler<AddPhoto> addPhotoHandler,
                            IQueryHandler<GetPhoto, PhotoDto> getPhotoHandler,
                            ICommandHandler<ChangePhotoOridinal> changePhotoOridinalHandler,
                            ICommandHandler<DeletePhoto> deletePhotoHandler)
    {
        _addPhotoHandler = addPhotoHandler;
        _getPhotoHandler = getPhotoHandler;
        _changePhotoOridinalHandler = changePhotoOridinalHandler;
        _deletePhotoHandler = deletePhotoHandler;
    }

    [HttpGet("{photoId:guid}")]
    public async Task<ActionResult<PhotoDto>> GetPhoto(Guid photoId)
    {
        var query = Authenticate(new GetPhoto { PhotoId = photoId});
        var photo = await _getPhotoHandler.HandleAsync(query);
        return photo;
    }

    [HttpPost]
    public async Task<ActionResult> Post(AddPhoto command)
    {
        command = Authenticate(command with {PhotoId = Guid.NewGuid()});
        await _addPhotoHandler.HandleAsync(command);

        var query = Authenticate(new GetPhoto { PhotoId = command.PhotoId});
        var photo = await _getPhotoHandler.HandleAsync(query);
        return CreatedAtAction(nameof(GetPhoto), new { command.PhotoId }, photo);
    }

    [HttpPatch("{photoId:guid}")]
    public async Task<ActionResult> Patch([FromRoute] Guid photoId, ChangePhotoOridinal command)
    {
        command = Authenticate(command with {PhotoId = photoId});
        await _changePhotoOridinalHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpDelete("{photoId:guid}")]
    public async Task<ActionResult> Delete(Guid photoId)
    {
        var command = Authenticate(new DeletePhoto(photoId));
        await _deletePhotoHandler.HandleAsync(command);
        return NoContent();
    }
}