using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class DeletePhotoHandler : ICommandHandler<DeletePhoto>
{
    private readonly IPhotoRepository _photoRepository;
    public DeletePhotoHandler(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
    }

    public async Task HandleAsync(DeletePhoto command)
    {
        var photo = await _photoRepository.GetByIdAsync(command.PhotoId);
        if (photo == null)
        {
            throw new PhotoNotExistsException(command.PhotoId);
        }
        await _photoRepository.DeleteAsync(photo);

        var fileExists = System.IO.File.Exists(photo.Path);
        if (fileExists)
        {
            try
            {
                System.IO.File.Delete(photo.Path);
            }
            catch (System.Exception)
            {
            }
        }
    }
}