using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Repositories;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Application.Storage;
using datingApp.Core.Repositories;
using FluentStorage.Blobs;

namespace datingApp.Application.Commands.Handlers;

public sealed class DeletePhotoHandler : ICommandHandler<DeletePhoto>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IBlobStorage _fileStorage;
    private readonly IDeletedEntityRepository _deletedEntityRepository;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public DeletePhotoHandler(IPhotoRepository photoRepository,
                            IBlobStorage fileStorageService,
                            IDeletedEntityRepository deletedEntityRepository,
                            IDatingAppAuthorizationService authorizationService)
    {
        _photoRepository = photoRepository;
        _fileStorage = fileStorageService;
        _deletedEntityRepository = deletedEntityRepository;
        _authorizationService = authorizationService;
    }

    public async Task HandleAsync(DeletePhoto command)
    {
        var photo = await _photoRepository.GetByIdAsync(command.PhotoId);
        if (photo == null)
        {
            if (await _deletedEntityRepository.ExistsAsync(command.PhotoId))
            {
                throw new PhotoAlreadyDeletedException(command.PhotoId);
            }
            else
            {
                throw new PhotoNotExistsException(command.PhotoId);
            }
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(command.AuthenticatedUserId, photo, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        var path = "TODO";
        var tasks = new List<Task>(){
            _fileStorage.DeleteAsync(path),
            _photoRepository.DeleteAsync(photo),
            _deletedEntityRepository.AddAsync(photo.Id),
        };
        await Task.WhenAll(tasks);
    }
}