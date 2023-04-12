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
[Route("photos")]
public class PhotosController : ControllerBase
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
        var photo = await _getPhotoHandler.HandleAsync(new GetPhoto { PhotoId = photoId});
        if (photo == null)
        {
            return NotFound();
        }
        return photo;
    }

    [HttpPost]
    public async Task<ActionResult> Post(AddPhoto command)
    {
        command = command with {PhotoId = Guid.NewGuid()};
        await _addPhotoHandler.HandleAsync(command);
        var photo = await _getPhotoHandler.HandleAsync(new GetPhoto { PhotoId = command.PhotoId});
        return CreatedAtAction(nameof(GetPhoto), new { command.PhotoId }, photo);
    }

    [HttpPatch]
    public async Task<ActionResult> Patch(ChangePhotoOridinal command)
    {
        await _changePhotoOridinalHandler.HandleAsync(command);
        var photo = await _getPhotoHandler.HandleAsync(new GetPhoto { PhotoId = command.PhotoId});
        return CreatedAtAction(nameof(GetPhoto), new { command.PhotoId }, photo);
    }

    [HttpDelete("{photoId:guid}")]
    public async Task<ActionResult> Delete(Guid photoId)
    {
        await _deletePhotoHandler.HandleAsync(new DeletePhoto(photoId));
        return NoContent();
    }
}