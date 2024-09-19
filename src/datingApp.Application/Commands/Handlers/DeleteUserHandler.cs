using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Repositories;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class DeleteUserHandler : ICommandHandler<DeleteUser>
{
    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IDeletedEntityRepository _deletedEntityRepository;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public DeleteUserHandler(IUserRepository userRepository, IFileStorageService fileStorageService, IDeletedEntityRepository deletedEntityRepository, IDatingAppAuthorizationService authorizationService)
    {
        _userRepository = userRepository;
        _fileStorageService = fileStorageService;
        _deletedEntityRepository = deletedEntityRepository;
        _authorizationService = authorizationService;
    }

    public async Task HandleAsync(DeleteUser command)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            if (await _deletedEntityRepository.ExistsAsync(command.UserId))
            {
                throw new UserAlreadyDeletedException(command.UserId);
            }
            else
            {
                throw new UserNotExistsException(command.UserId);
            }
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(command.AuthenticatedUserId, user, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        foreach (var photo in user.Photos)
        {
            _fileStorageService.DeleteFile(photo.Id.ToString());
        }

        await _userRepository.DeleteAsync(user);
        await _deletedEntityRepository.AddAsync(user.Id);
    }
}