using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
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

public sealed class DeleteUserHandler : ICommandHandler<DeleteUser>
{
    private readonly IUserRepository _userRepository;
    private readonly IBlobStorage _fileStorage;
    private readonly IDeletedEntityRepository _deletedEntityRepository;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public DeleteUserHandler(IUserRepository userRepository,
                            IBlobStorage fileStorageService,
                            IDeletedEntityRepository deletedEntityRepository,
                            IDatingAppAuthorizationService authorizationService)
    {
        _userRepository = userRepository;
        _fileStorage = fileStorageService;
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

        var paths = new List<string>();
        foreach (var photo in user.Photos)
        {
            //pass
        }
        
        var tasks = new List<Task>()
        {
            _fileStorage.DeleteAsync(paths),
            _userRepository.DeleteAsync(user),
            _deletedEntityRepository.AddAsync(user.Id),
        };
        await Task.WhenAll(tasks);
    }
}