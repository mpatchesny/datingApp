using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Repositories;
using datingApp.Application.Services;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class DeletePhotoHandler : ICommandHandler<DeletePhoto>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IFileRepository _fileRepository;
    public DeletePhotoHandler(IPhotoRepository photoRepository,
                              IFileRepository fileRepository)
    {
        _photoRepository = photoRepository;
        _fileRepository = fileRepository;
    }

    public async Task HandleAsync(DeletePhoto command)
    {
        var photo = await _photoRepository.GetByIdAsync(command.PhotoId);
        if (photo == null)
        {
            throw new PhotoNotExistsException(command.PhotoId);
        }

        await Task.WhenAll(
            _photoRepository.DeleteAsync(photo),
            _fileRepository.DeleteAsync(photo.Id)
        );
    }
}