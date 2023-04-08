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

    [HttpGet("{photoId:int}")]
    public async Task<ActionResult<PhotoDto>> Get(int photoId)
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
        await _addPhotoHandler.HandleAsync(command);
        return CreatedAtAction(nameof(Get), new { photoId = 1 });
    }

    [HttpPatch]
    public async Task<ActionResult> Patch(ChangePhotoOridinal command)
    {
        await _changePhotoOridinalHandler.HandleAsync(command);
        return CreatedAtAction(nameof(Get), new { photoId = 1 });
    }

    [HttpDelete("{photoId:int}")]
    public async Task<ActionResult> Delete(int photoId)
    {
        await _deletePhotoHandler.HandleAsync(new DeletePhoto(photoId));
        return Ok();
    }
}