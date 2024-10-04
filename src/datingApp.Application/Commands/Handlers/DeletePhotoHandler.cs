using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Repositories;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Application.Storage;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class DeletePhotoHandler : ICommandHandler<DeletePhoto>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IDeletedEntityRepository _deletedEntityRepository;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public DeletePhotoHandler(IPhotoRepository photoRepository, IFileStorageService fileStorageService, IDeletedEntityRepository deletedEntityRepository, IDatingAppAuthorizationService authorizationService)
    {
        _photoRepository = photoRepository;
        _fileStorageService = fileStorageService;
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

        _fileStorageService.DeleteFile(photo.Id.ToString());
        await _photoRepository.DeleteAsync(photo);
        await _deletedEntityRepository.AddAsync(photo.Id);
    }
}