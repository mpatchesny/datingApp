using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Services;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class DeletePhotoHandler : ICommandHandler<DeletePhoto>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IPhotoService _photoService;
    private readonly IFileStorage _fileStorage;
    public DeletePhotoHandler(IPhotoRepository photoRepository,
                                IPhotoService photoService,
                                IFileStorage fileStorage)
    {
        _photoRepository = photoRepository;
        _photoService = photoService;
        _fileStorage = fileStorage;
    }

    public async Task HandleAsync(DeletePhoto command)
    {
        var photo = await _photoRepository.GetByIdAsync(command.PhotoId);
        if (photo == null)
        {
            throw new PhotoNotExistsException(command.PhotoId);
        }

        Task[] tasks = new Task[2];
        tasks[0] = _photoRepository.DeleteAsync(photo);
        tasks[1] = _fileStorage.DeleteFileAsync(photo.Path);
        Task.WaitAll(tasks);
    }
}