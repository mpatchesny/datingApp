using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Application.Storage;
using datingApp.Core.Repositories;
using FluentStorage.Blobs;

namespace datingApp.Application.Commands.Handlers;

public sealed class DeletePhotoHandler : ICommandHandler<DeletePhoto>
{
    private readonly IUserRepository _userRepository;
    private readonly IBlobStorage _fileStorage;
    private readonly IDeletedEntityService _deletedEntityService;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public DeletePhotoHandler(IUserRepository userRepository,
                            IBlobStorage fileStorageService,
                            IDeletedEntityService deletedEntityRepository,
                            IDatingAppAuthorizationService authorizationService)
    {
        _userRepository = userRepository;
        _fileStorage = fileStorageService;
        _deletedEntityService = deletedEntityRepository;
        _authorizationService = authorizationService;
    }

    public async Task HandleAsync(DeletePhoto command)
    {
        var user = await _userRepository.GetByPhotoIdAsync(command.PhotoId);
        if (user == null)
        {
            if (await _deletedEntityService.ExistsAsync(command.PhotoId))
            {
                throw new PhotoAlreadyDeletedException(command.PhotoId);
            }
            else
            {
                throw new PhotoNotExistsException(command.PhotoId);
            }
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(command.AuthenticatedUserId, user, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        var photo = user.Photos.FirstOrDefault(p => p.Id.Value == command.PhotoId);
        var path = $"{photo.Id}.{photo.Extension}";
        user.RemovePhoto(command.PhotoId);

        var tasks = new List<Task>(){
            _fileStorage.DeleteAsync(path),
            _userRepository.UpdateAsync(user)
        };
        await Task.WhenAll(tasks);
        await _deletedEntityService.AddAsync(photo.Id, "photo", DateTime.UtcNow);
    }
}