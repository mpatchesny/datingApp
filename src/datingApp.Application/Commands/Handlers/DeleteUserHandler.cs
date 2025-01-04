using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
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
    private readonly ISwipeRepository _swipeRepository;
    private readonly IDeletedEntityService _deletedEntityService;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public DeleteUserHandler(IUserRepository userRepository,
                            IBlobStorage fileStorageService,
                            IDeletedEntityService deletedEntityRepository,
                            IDatingAppAuthorizationService authorizationService,
                            ISwipeRepository swipeRepository)
    {
        _userRepository = userRepository;
        _fileStorage = fileStorageService;
        _deletedEntityService = deletedEntityRepository;
        _authorizationService = authorizationService;
        _swipeRepository = swipeRepository;
    }

    public async Task HandleAsync(DeleteUser command)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            if (await _deletedEntityService.ExistsAsync(command.UserId))
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

        var paths = user.Photos.Select(photo => $"{photo.Id}.{photo.Extension}").ToList();

        var tasks = new List<Task>()
        {
            _fileStorage.DeleteAsync(paths),
            _userRepository.DeleteAsync(user),
        };
        await Task.WhenAll(tasks);
        await _swipeRepository.DeleteUserSwipes(user.Id);
        await _deletedEntityService.AddAsync(user.Id);
    }
}