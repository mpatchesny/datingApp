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
    public PhotosController(ICommandHandler<AddPhoto> addPhotoHandler,
                            IQueryHandler<GetPhoto, PhotoDto> getPhotoHandler)
    {
        _addPhotoHandler = addPhotoHandler;
        _getPhotoHandler = getPhotoHandler;
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
        return CreatedAtAction(nameof(Get), new { id = 1 });
    }
}