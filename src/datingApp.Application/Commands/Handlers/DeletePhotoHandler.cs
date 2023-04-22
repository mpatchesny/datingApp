using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class DeletePhotoHandler : ICommandHandler<DeletePhoto>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IPhotoService _photoService;
    public DeletePhotoHandler(IPhotoRepository photoRepository, IPhotoService photoService)
    {
        _photoRepository = photoRepository;
        _photoService = photoService;
    }

    public async Task HandleAsync(DeletePhoto command)
    {
        var photo = await _photoRepository.GetByIdAsync(command.PhotoId);
        if (photo == null)
        {
            throw new PhotoNotExistsException(command.PhotoId);
        }
        await _photoRepository.DeleteAsync(photo);

        _photoService.DeletePhoto(photo.Path);
    }
}