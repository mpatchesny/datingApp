using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class ChangePhotoOridinalHandler : ICommandHandler<ChangePhotoOridinal>
{
    private readonly IUserRepository _userRepository;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public ChangePhotoOridinalHandler(IUserRepository userRepository, 
                                      IDatingAppAuthorizationService authorizationService)
    {
        _userRepository = userRepository;
        _authorizationService = authorizationService;
    }

    public async Task HandleAsync(ChangePhotoOridinal command)
    {
        var user = await _userRepository.GetByPhotoIdAsync(command.PhotoId);
        if (user == null)
        {
            throw new PhotoNotExistsException(command.PhotoId);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(command.AuthenticatedUserId, user, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        user.ChangeOridinal(command.PhotoId, command.NewOridinal);
        await _userRepository.UpdateAsync(user);
    }
}